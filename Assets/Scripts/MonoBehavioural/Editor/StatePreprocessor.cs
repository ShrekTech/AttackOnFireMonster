using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

namespace MonoBehavioural
{

    public class StateAssetPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (importedAssets.Length > 20)
            {
                Debug.LogWarning("Lots of assets imported at once. Skipping state-machine code generator.");
                File.WriteAllText("Temp/importedScripts.txt", string.Empty);
                return;
            }

            List<string> importedScripts = new List<string>();

            foreach (var path in importedAssets)
            {
                var importer = AssetImporter.GetAtPath(path) as MonoImporter;
                if (importer)
                {
                    if (importer.userData == StateCodeGenerator.GeneratorUserData)
                    {
                        string generatedAssetPath = StateCodeGenerator.GetGeneratedScriptPath(importer);
                        if (!string.IsNullOrEmpty(generatedAssetPath))
                        {
                            Debug.Log(string.Format("Clearing statemachine code for {0}", path));
                            File.WriteAllText(generatedAssetPath, string.Empty);
                        }
                    }
                    importedScripts.Add(path);
                }
            }

            if (importedScripts.Count > 0)
            {
                string importedScriptsText = string.Join(",", importedScripts.ToArray()) + ",";
                File.AppendAllText("Temp/importedScripts.txt", importedScriptsText);
            }
        }
    }

    public static class StateCodeGenerator
    {
        class CompareMethodNames : IEqualityComparer<MethodInfo>
        {
            public bool Equals(MethodInfo x, MethodInfo y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(MethodInfo obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        //static CompareMethodNames methodNameComparer = new CompareMethodNames();

        public static readonly string GeneratorUserData = "StateMachineGenerator";
        public static readonly string GeneratedScriptUserData = "StateMachineGeneratorProduct";

        public static string GetGeneratedScriptPath(MonoImporter importer)
        {
            return Path.GetDirectoryName(importer.assetPath) + "/" + Path.GetFileNameWithoutExtension(importer.assetPath) + "_States.cs";
        }

        [DidReloadScripts(-1)]
        public static void GenerateScripts()
        {
            List<string> generatedFiles = new List<string>();

            if (!File.Exists("Temp/importedScripts.txt"))
                return;

            string importedScriptsText = File.ReadAllText("Temp/importedScripts.txt");
            string[] importedScripts = importedScriptsText.Split(',');

            var uniquePaths = importedScripts.Distinct();

            foreach (var path in uniquePaths)
            {
                var importer = AssetImporter.GetAtPath(path) as MonoImporter;
                if (importer)
                {
                    string generatedFilePath;
                    bool generated = StateCodeGenerator.GenerateCode(path, out generatedFilePath);
                    if (generated)
                        generatedFiles.Add(generatedFilePath);
                }
            }

            File.WriteAllText("Temp/importedScripts.txt", string.Empty);

            if (generatedFiles.Count > 0)
            {
                Debug.Log(generatedFiles.Aggregate("Generated: ", (current, next) => string.Format("{0}\n{1}", current, next)));
                AssetDatabase.Refresh();
            }
        }

        public static bool GenerateCode(string path, out string stateScriptPath)
        {
            stateScriptPath = null;
            try
            {
                var generatorScriptImporter = AssetImporter.GetAtPath(path) as MonoImporter;
                if (!generatorScriptImporter)
                {
                    Debug.LogError(path + " doesn't use MonoImporter");
                    return false;
                }
                // If this script is already known to be generated, don't bother loading it
                if (generatorScriptImporter.userData == GeneratedScriptUserData)
                    return false;

                var scriptAsset = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript)) as MonoScript;
                if (scriptAsset)
                {
                    var scriptClass = scriptAsset.GetClass();
                    if (scriptClass == null)
                    {
                        //Debug.Log("No class in " + path);
                        return false;
                    }

                    var stateTypes = scriptClass.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(entry => entry.GetCustomAttributes(typeof(StateAttribute), true).Length > 0);

                    if (!stateTypes.Any())
                        return false;

                    stateScriptPath = GetGeneratedScriptPath(generatorScriptImporter);

                    // Set the userData
                    generatorScriptImporter.userData = GeneratorUserData;
                    EditorUtility.SetDirty(scriptAsset);
                    {
                        var scriptImporter = AssetImporter.GetAtPath(stateScriptPath) as MonoImporter;
                        if (scriptImporter)
                        {
                            scriptImporter.userData = GeneratedScriptUserData;
                        }
                    }

                    string scriptText = string.Empty;

                    {
                        // Before generating any code, a bunch of stuff is checked in the provided types
                        //  so that more helpful error messages can be given, and no time is wasted on
                        //  classes that wont work.

                        var scriptClassMethods = scriptClass.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                        var scriptClassMethodNames = new HashSet<string>(scriptClassMethods.Select(entry => entry.Name));

                        Dictionary<string, MethodInfo> defaultMethodsByCallingName = new Dictionary<string, MethodInfo>();
                        for (int i = 0; i < scriptClassMethods.Length; ++i)
                        {
                            var method = scriptClassMethods[i];
                            if (!method.IsGenericMethodDefinition)
                            {
                                var attributes = method.GetCustomAttributes(typeof(DefaultHandlerAttribute), true);
                                if (method.Name.StartsWith("Default_") || attributes.Length > 0)
                                {
                                    string eventName;
                                    if (!GetEventName(attributes, out eventName))
                                    {
                                        eventName = method.Name.Remove(0, "Default_".Length);
                                    }
                                    defaultMethodsByCallingName.Add(eventName, method);
                                }
                            }
                        }

                        List<string> allStateMethodNames = new List<string>();

                        // Add the CALLING NAMES for the default methods (not their actual method names, but the names that may match state methods)
                        allStateMethodNames.AddRange(defaultMethodsByCallingName.Keys);

                        // Make a list that contains method-info representing each state-method signature
                        //  The list will contain one method with each name
                        //  Also checks that the signatures are compatible for state methods with the same name
                        List<MethodInfo> representativeStateMethods = new List<MethodInfo>();
                        {
                            Dictionary<string, MethodInfo> stateMethodsByName = new Dictionary<string, MethodInfo>(defaultMethodsByCallingName);

                            var allStateMethods = stateTypes.SelectMany(entry =>
                            {
                                return entry.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                    .Where(method => !method.IsGenericMethodDefinition)
                                    .Where(method => method.GetCustomAttributes(typeof(IgnoreAttribute), false).Length == 0)
                                    .Where(method => method.DeclaringType.GetCustomAttributes(typeof(StateAttribute), true).Length > 0);
                            });
                            foreach (var method in allStateMethods)
                            {
                                MethodInfo alreadyKnownMethod;
                                if (stateMethodsByName.TryGetValue(method.Name, out alreadyKnownMethod))
                                {
                                    if (method.ReturnType != alreadyKnownMethod.ReturnType)
                                    {
                                        Debug.LogError("Incompatible state methods for " + method.Name + ": " + GetClassQualifiedMethodName(method) + " doesn't have the same return type as " + GetClassQualifiedMethodName(alreadyKnownMethod));
                                        return false;
                                    }
                                    else if (!method.GetParameters().Select(entry => entry.ParameterType).SequenceEqual(alreadyKnownMethod.GetParameters().Select(entry => entry.ParameterType)))
                                    {
                                        Debug.LogError("Incompatible state methods for " + method.Name + ": " + GetClassQualifiedMethodName(method) + " doesn't have the same parameter types as " + GetClassQualifiedMethodName(alreadyKnownMethod));
                                        return false;
                                    }
                                }
                                else
                                {
                                    // A new method name has been encountered (this method becomes the representative of that name)
                                    stateMethodsByName.Add(method.Name, method);

                                    representativeStateMethods.Add(method);
                                    allStateMethodNames.Add(method.Name);
                                }
                            }
                        }

                        // Check the validity of the state method names used
                        {
                            if (scriptClassMethodNames.Contains("Awake"))
                            {
                                Debug.LogError(string.Format(
                                    "Can't generate state machine for {0}: The class declares a method with the name \"Awake\", which is reserved for use by the generated code.",
                                    scriptAsset.name));
                            }

                            // Check the state method names against any regular
                            //  old methods declared in the state machine class
                            foreach (var method in representativeStateMethods)
                            {
                                // Awake is always dissallowed since that is used to initialise the state machine
                                if (method.Name == "Awake")
                                {
                                    Debug.LogError(string.Format(
                                        "Can't generate state machine for {0}: The state {1} declares a method with the name \"Awake\", which is reserved for use by the generated code.",
                                        scriptAsset.name, method.DeclaringType.Name));
                                    return false;
                                }

                                if (scriptClassMethodNames.Contains(method.Name))
                                {
                                    Debug.LogError(string.Format(
                                        "Can't generate state machine for {0}: The state {1} declares a method called {2} which is already declared in the state machine class {0}.",
                                        scriptAsset.name, method.DeclaringType.Name, method.Name));
                                    return false;
                                }
                            }
                        }

                        scriptText +=
@"// Generated file (yey). Based on" + path +
@"
using UnityEngine;
using System.Collections.Generic;
";

                        scriptText +=
                            string.Format(
@"public partial class {0}",
                            scriptClass.Name);

                        // Add the interfaces for the state types
                        var interfaces = stateTypes.SelectMany(stateType => stateType.GetInterfaces()).Distinct();
                        if (interfaces.Any())
                        {
                            string interfacesText = string.Join(", ", interfaces.Cast<string>().ToArray());
                            scriptText += string.Format(" : {0}", interfacesText);
                        }

                        scriptText +=
@"{
";

                        // Generate the event handlers / public methods
                        foreach (var entry in defaultMethodsByCallingName)
                        {
                            scriptText = EmitStateMethodCaller(scriptText, entry.Key, entry.Value, true);
                        }
                        foreach (var method in representativeStateMethods)
                        {
                            // Enter and exit are special methods called by the state machine on transitions
                            //  The methods for calling these are declared separately from the regular state
                            //  method callers
                            if (method.Name == "Enter" || method.Name == "Exit")
                            {
                                continue;
                            }

                            bool hasDefault = defaultMethodsByCallingName.ContainsKey(method.Name);

                            scriptText = EmitStateMethodCaller(scriptText, method.Name, method, hasDefault);
                        }

                        // Pre-generate all the type names for the callback fields
                        Dictionary<string, string> delegateTypesByMethodName = new Dictionary<string, string>();
                        foreach (var method in representativeStateMethods)
                        {
                            delegateTypesByMethodName.Add(method.Name, MakeDelegateTypeName(method));
                        }
                        foreach (var entry in defaultMethodsByCallingName)
                        {
                            var stateMethodName = entry.Key;
                            var method = entry.Value;
                            delegateTypesByMethodName[stateMethodName] = MakeDelegateTypeName(method);
                        }

                        // A type to store the delegates for each state
                        {
                            scriptText +=
@"
    sealed class StateMethods {";
                            foreach (var methodName in allStateMethodNames)
                            {
                                scriptText +=
@"
        public " + delegateTypesByMethodName[methodName] + " " + methodName + @";";
                            }
                            scriptText +=
@"
    }
";
                        }

                        {
                            scriptText +=
@"
    Stack<StateMethods> stateStack = new Stack<StateMethods>();
    StateMethods currentState { get { return stateStack.Peek(); } }
    Dictionary<string, StateMethods> states = new Dictionary<string, StateMethods>();
    StateMethods noState;
";

                            bool atLeastOneStateHasEnterCallback = allStateMethodNames.Contains("Enter");
                            bool atLeastOneStateHasExitCallback = allStateMethodNames.Contains("Exit");

                            // ChangeState method
                            if (atLeastOneStateHasExitCallback)
                                scriptText +=
@"    bool __exitingState;";

                            scriptText +=
@"

    public void ChangeState<T>() {
        this.ChangeState(typeof(T));
    }

    public void PushState<T>() {
        this.PushState(typeof(T));
    }
";
                            System.Action emitFailIfExiting = () =>
                            {
                                scriptText +=
@"
        if (__exitingState)
        {
            Debug.LogError(""Can't change state while exiting"");
            return;
        }";
                            };

                            System.Action<string, string> emitChangeStateMethod;

                            emitChangeStateMethod = (methodName, setStateCode) =>
                            {
                                scriptText +=
@"
    public void " + methodName + @"(System.Type stateIdentifier) {";
                                if (atLeastOneStateHasExitCallback)
                                    emitFailIfExiting();

                                scriptText +=
@"
        StateMethods nextState;
        bool validState = GetState(stateIdentifier, out nextState);

        if (!validState)
            return;

        if (nextState == stateStack.Peek())
            return;
";
                                if (atLeastOneStateHasExitCallback)
                                {
                                    scriptText +=
@"
        CallExit(currentState);
";
                                }
                                scriptText +=
@"
        " + setStateCode + @"
";
                                if (atLeastOneStateHasEnterCallback)
                                {
                                    scriptText +=
@"
        CallEnter(currentState);";
                                }
                                scriptText +=
@"
    }
";
                            };

                            // Emit regular old go-to-state state method
                            emitChangeStateMethod("ChangeState",
@"stateStack.Clear();
        if (nextState != noState)
            stateStack.Push(noState);
        stateStack.Push(nextState);");
                            // Pushing states on to the stack
                            emitChangeStateMethod("PushState",
@"stateStack.Push(nextState);");

                            // Pop state
                            scriptText +=
@"
    public void PopState() {";
                            if (atLeastOneStateHasExitCallback)
                                emitFailIfExiting();

                            scriptText +=
@"
        if (stateStack.Count <= 1)
        {
            Debug.LogError(""No more states to pop."");
            return;
        }
";
                            if (atLeastOneStateHasExitCallback)
                            {
                                scriptText +=
@"
        CallExit(currentState);
";
                            }
                            scriptText +=
@"
        stateStack.Pop();
";
                            if (atLeastOneStateHasEnterCallback)
                            {
                                scriptText +=
@"
        CallEnter(currentState);";
                            }
                            scriptText +=
@"
    }
";

                            scriptText +=
@"
    private void CallEnter(StateMethods state)
    {
        if (state.Enter != null)
        {
            try
            {
                state.Enter();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
";
                            scriptText +=
@"
    private void CallExit(StateMethods state)
    {
        if (state.Exit != null)
        {
            __exitingState = true;
            try
            {
                state.Exit();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
            __exitingState = false;
        }
    }
";

                            scriptText +=
@"
    public string GetActiveState()
    {
        foreach (var entry in states)
        {
            if (entry.Value == currentState)
            {
                return entry.Key;
            }
        }
        return ""No State"";
    }
";

                            scriptText +=
@"
    private bool GetState(System.Type stateIdentifier, out StateMethods nextState)
    {
        if (stateIdentifier != null)
        {
            if (!states.TryGetValue(stateIdentifier.Name, out nextState))
            {
                Debug.LogError(""Tried to change state to "" + stateIdentifier.Name + "", which is not defined in " + scriptClass.Name + @"."");
                return false;
            }
        }
        else
        {
            nextState = noState;
        }

        return true;
    }

";
                        }

                        Dictionary<System.Type, System.Type> replacementStates = new Dictionary<System.Type, System.Type>();
                        foreach (var stateType in stateTypes)
                        {
                            var stateAttributes = stateType.GetCustomAttributes(typeof(StateAttribute), true).Cast<StateAttribute>();
                            if (stateAttributes.Any(attribute => attribute.ReplaceBase))
                            {
                                replacementStates.Add(stateType, stateType.BaseType);
                            }
                            //if (stateAttributes.Any(attribute => attribute.ReplaceState != null))
                            //{
                            //    replacementStates.Add(stateType, attribute.ReplaceState);
                            //}
                        }
                        // TODO: tidy up ^ these v
                        {
                            var replacedStates = stateTypes
                                .Where(entry => entry.GetCustomAttributes(typeof(StateAttribute), true).Cast<StateAttribute>().Any(attribute => attribute.ReplaceBase))
                                .Select(entry => entry.BaseType);

                            stateTypes = stateTypes.Except(replacedStates);
                        }

                        foreach (var stateType in stateTypes)
                        {
                            string instanceVariableName = MakeInstanceVariableName(stateType);

                            // Declare the state instance
                            if (stateType.GetCustomAttributes(typeof(System.SerializableAttribute), false).Length > 0)
                                scriptText +=
@"    [SerializeField]
";
                            scriptText += string.Format(
@"    {0} {1};
", stateType.Name, instanceVariableName);
                        }

                        scriptText +=
@"
    void Awake() {
";
                        // Default method callbacks (callbacks in this class)
                        if (defaultMethodsByCallingName.Count > 0)
                        {
                            scriptText +=
@"        {
            noState = new StateMethods();
";
                            foreach (var entry in defaultMethodsByCallingName)
                            {
                                var method = entry.Value;
                                string mappedMethodName = entry.Key;

                                var defaultHandlerAttribute = method.GetCustomAttributes(typeof(DefaultHandlerAttribute), true);
                                if (defaultHandlerAttribute.Length > 0)
                                {
                                    mappedMethodName = ((DefaultHandlerAttribute)defaultHandlerAttribute[0]).EventName;
                                }
                                scriptText += string.Format(
@"            noState.{1} = System.Delegate.CreateDelegate(typeof({0}), this, ""{2}"") as {0};
",
                                    delegateTypesByMethodName[mappedMethodName], mappedMethodName, method.Name);
                            }
                        }
                        // Push the default state
                        scriptText +=
@"            stateStack.Push(noState);
    }";
                        // Method callbacks for each state
                        foreach (var stateType in stateTypes)
                        {
                            string instanceVariableName = MakeInstanceVariableName(stateType);

                            scriptText += string.Format(
@"
        if ({0} == null) // Could be serialised in the container (if so, don't want to replace that instance)
            {0} = new {1}();
",
                                instanceVariableName, stateType.Name);

                            if (stateType.GetField("container") != null)
                            {
                                scriptText += string.Format(
@"        {0}.container = this;",
                                    instanceVariableName);
                            }

                            // Set up the callbacks for this state
                            scriptText +=
@"
        {
            StateMethods stateMethods = new StateMethods();
";

                            var methods = stateType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                .Where(entry => !entry.IsGenericMethodDefinition)
                                .Where(entry => entry.GetCustomAttributes(typeof(IgnoreAttribute), true).Length == 0)
                                .Where(entry => entry.DeclaringType.GetCustomAttributes(typeof(StateAttribute), true).Length > 0);
                            foreach (var method in methods)
                            {
                                scriptText += string.Format(
@"                stateMethods.{1} = System.Delegate.CreateDelegate(typeof({0}), {2}, ""{1}"") as {0};
",
                                    delegateTypesByMethodName[method.Name], method.Name, instanceVariableName);
                            }
                            // Add the state to the collection
                            scriptText += string.Format(
@"
            states.Add(""{0}"", stateMethods);
", stateType.Name);
                            // Add references to overridden (replaced) states
                            {
                                System.Type replacedState;
                                if (replacementStates.TryGetValue(stateType, out replacedState))
                                {
                                    scriptText +=
@"            // Replaced state
            states.Add(""" + replacedState.Name + @""", stateMethods);
";
                                }
                            }
                            scriptText +=
@"        }
";
                        }

                        // Set the initial state
                        var initialStateAttributes = scriptClass.GetCustomAttributes(typeof(InitialStateAttribute), true);
                        if (initialStateAttributes.Length > 0)
                        {
                            var initialStateAttribute = (InitialStateAttribute)initialStateAttributes[0];
                            if (initialStateAttributes.Length > 1)
                            {
                                Debug.LogWarning(string.Format("More than one initial state defined in {0}. Only one will be used. Using {1}", scriptAsset.name, initialStateAttribute.Identifier.Name));
                            }
                            if (initialStateAttribute.Identifier.DeclaringType == scriptClass)
                            {
                                scriptText += string.Format(
@"
        this.ChangeState(typeof({0}));
", initialStateAttribute.Identifier.Name);
                            }
                            else
                            {
                                Debug.LogError(string.Format("The initial state {1} set in {0} is not a nested type of {0}. The initial state will not be set.", scriptAsset.name, initialStateAttribute.Identifier.Name));
                            }
                        }


                        scriptText +=
@"    }
";
                        // End of class
                        scriptText +=
@"}
";
                    }

                    File.WriteAllText(stateScriptPath, scriptText);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
            return false;
        }

        private static string EmitStateMethodCaller(string scriptText, string methodName, MethodInfo method, bool hasDefault)
        {
            bool hasReturn = method.ReturnType != typeof(void);

            string parameters = string.Join(", ", method.GetParameters()
                .Select(entry => entry.ToString())
                .ToArray());

            string parametersForCall = string.Join(", ", method.GetParameters()
                .Select(entry => entry.Name)
                .ToArray());

            scriptText +=
@"    ";
            if (method.IsPublic)
                scriptText +=
@"public ";
            scriptText += string.Format(
@"{0} {1}({2}) {{", hasReturn ? method.ReturnType.ToString() : "void", methodName, parameters);

            System.Action<string> emitExecuteStateHandler;
            if (hasReturn)
            {
                emitExecuteStateHandler = (methodStructVariableName) =>
                {
                    scriptText +=
@"
        if (" + methodStructVariableName + "." + methodName + @" != null) {
            return " + methodStructVariableName + "." + methodName + @"(" + parametersForCall + @");
        }";
                };
            }
            else
            {
                emitExecuteStateHandler = (methodStructVariableName) =>
                {
                    scriptText +=
@"
        if (" + methodStructVariableName + "." + methodName + @" != null) {
            " + methodStructVariableName + "." + methodName + @"(" + parametersForCall + @");
            return;
        }";
                };
            }
            emitExecuteStateHandler("currentState");

            // At this point the method has failed to execute a handler
            //  if there's a default available, it'll execute that
            //  otherwise, and if a return value was expected, it'll warn
            //   and return the default for the return type
            if (hasDefault)
            {
                emitExecuteStateHandler("noState");
            }
            if (hasReturn)
            {
                scriptText +=
@"
        Debug.LogWarning(""No method for " + methodName + @" in current state."");
        return default(" + method.ReturnType.FullName + @");";
            }
            scriptText +=
@"
    }
";
            return scriptText;
        }

        private static string MakeDelegateTypeName(MethodInfo method)
        {
            string delegateType;

            string parameterTypes = string.Join(", ", method.GetParameters()
                    .Select(entry => entry.ParameterType.FullName)
                    .ToArray());
            if (method.ReturnType == typeof(void))
            {
                delegateType = string.Format("System.Action{0}",
                    parameterTypes.Length > 0 ? string.Format("<{0}>", parameterTypes) : string.Empty);
            }
            else
            {
                delegateType = string.Format("System.Func<{0}{1}>",
                    parameterTypes.Length > 0 ? string.Format("{0}, ", parameterTypes) : string.Empty, method.ReturnType.FullName);
            }

            return delegateType;
        }
        private static string MakeInstanceVariableName(System.Type stateType)
        {
            string instanceVariableName = "_" + stateType.Name;
            return instanceVariableName;
        }

        private static string GetClassQualifiedMethodName(MethodInfo method)
        {
            string parameters = string.Join(", ", method.GetParameters().Select(entry => entry.ParameterType.Name).ToArray());
            string classQualifiedName = string.Format("{0} {1}.{2}({3})", method.ReturnType.Name, method.DeclaringType, method.Name, parameters);
            return classQualifiedName;
        }

        private static bool GetEventName(object[] attributes, out string eventName)
        {
            if (attributes.Length > 0)
            {
                var defaultHandlerAttribute = (DefaultHandlerAttribute)attributes[0];
                if (!string.IsNullOrEmpty(defaultHandlerAttribute.EventName))
                {
                    eventName = defaultHandlerAttribute.EventName;
                    return true;
                }
            }
            eventName = null;
            return false;
        }
    }

}
