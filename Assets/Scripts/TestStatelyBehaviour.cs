using UnityEngine;
using System.Collections;

using MonoBehavioural;

[InitialState(typeof(TestStatelyBehaviour.Airborne))]
public partial class TestStatelyBehaviour : MonoBehaviour
{
    [SerializeField]
    protected float fart;

    [DefaultHandler("Start")]
    void Garbald()
    {
        Debug.Log("Default Start");
    }

    // Default handler for boing, in attribute-free style ("Default_...")
    public void Default_Boing()
    {
        Debug.Log("Boing");
    }

    void Default_OnCollisionEnter(Collision c)
    {
        Debug.Log("Default OnCollisionEnter");
        this.ChangeState<Grounded>();
    }

    void Default_OnCollisionExit(Collision c)
    {
        Debug.Log("Default OnCollisionExit");
        this.ChangeState<Airborne>();
    }

    void Default_OnGUI()
    {
        GUILayout.BeginArea(new Rect(40, 100, 250, 600));
        GUILayout.Label(new GUIContent("Default OnGUI. Active state: " + this.GetActiveState()));
        if (GUILayout.Button(new GUIContent("Clear states")))
            this.ChangeState(null);
        if (GUILayout.Button(new GUIContent("Pop state")))
            this.PopState();
        GUILayout.EndArea();
    }

    [State]
    [System.Serializable]
    public class Airborne
    {
        [System.NonSerialized]
        public TestStatelyBehaviour container;

        [SerializeField]
        float blort;

        void Enter()
        {
            container.fart += 1;
            blort += 1;
        }

        void Boing()
        {
            Debug.Log("Airborne boing");
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(40, 100, 250, 600));
            GUILayout.Label(new GUIContent("Airborne"));
            if (GUILayout.Button(new GUIContent("Go to Woof")))
                container.ChangeState<Woof>();
            if (GUILayout.Button(new GUIContent("Push Arf")))
                container.PushState(typeof(Arf));
            GUILayout.EndArea();
        }

        void OnCollisionEnter(Collision collision)
        {
            container.ChangeState(typeof(Grounded));
        }
    }

    [State]
    protected class Grounded
    {
        public TestStatelyBehaviour container;

        void Enter()
        {
            container.fart += 1;
        }

        void Update()
        {
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(40, 100, 250, 600));
            GUILayout.Label(new GUIContent("Grounded"));
            GUILayout.EndArea();
        }

        public bool HasTheCool()
        {
            return true;
        }

        void OnCollisionExit(Collision collision)
        {
            container.ChangeState(typeof(Airborne));
        }
    }

    [State]
    protected class Woof
    {
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(40, 100, 250, 600));
            GUILayout.Label(new GUIContent("Woof"));
            GUILayout.EndArea();
        }

        protected virtual void Enter()
        {
            Debug.Log("Enter Woof");
        }

        protected void Exit()
        {
            Debug.Log("Exit Woof");
        }

        // Suppress collision handlers
        protected void OnCollisionEnter(Collision collision)
        {
        }
        protected void OnCollisionExit(Collision collision)
        {
        }
    }

    [State(ReplaceBase=true)]
    [System.Serializable]
    protected class Arf : Woof
    {
        public TestStatelyBehaviour container;

        [SerializeField]
        bool gruff;

        protected override void Enter()
        {
            base.Enter();
            Debug.Log("Enter Arf");
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(40, 100, 250, 600));
            GUILayout.Label(new GUIContent("Arf"));
            if (GUILayout.Button(new GUIContent("Push Ruff")))
                container.PushState<Ruff>();
            if (GUILayout.Button(new GUIContent("Pop State")))
                container.PopState();
            GUILayout.EndArea();
        }
    }

    [State]
    protected class Ruff : Woof
    {
        public TestStatelyBehaviour container;

        protected override void Enter()
        {
            base.Enter();
            Debug.Log("Enter Ruff");
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(40, 100, 250, 600));
            GUILayout.Label(new GUIContent("Ruff"));
            if (GUILayout.Button(new GUIContent("Push Arf")))
                container.PushState<Arf>();
            if (GUILayout.Button(new GUIContent("Pop State")))
                container.PopState();
            GUILayout.EndArea();
        }
    }
}