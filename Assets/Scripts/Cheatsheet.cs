using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Cheatsheet : MonoBehaviour
{
    //Variables & Data Types
    int x = 5;
    float y = 5.5f;
    double z = 10.5;
    bool isActive = true;
    char grade = 'A';
    string name = "John";


    // Constants & Readonly
    const double Pi = 3.14; // compile-time constant
    readonly int id = 100;  // runtime constant (can only be assigned in constructor)


    #region MonoBehaviour Lifecycle Methods

    void Awake() { }         // Called when object is initialized (before Start)
    void OnEnable() { }      // Called when the object becomes active
    void Start() { }         // Called before the first frame update
    void Update() { }        // Called every frame
    void FixedUpdate() { }   // Called at fixed intervals (physics)
    void LateUpdate() { }    // Called after Update
    void OnDisable() { }
    void OnDestroy() { }

    #endregion



    private async Task StartAsync()
    {
        #region  Control Structures
        //Conditionals

        if (x > 10) { }
        else if (x == 10) { }
        else { }

        switch (x)
        {
            case 1:
                break;
            default:
                break;
        }
        #endregion

        #region Loops

        for (int i = 0; i < 10; i++) { }

        while (x > 10) { }

        do { } while (x > 10);

        Stack<int> numberStack = new Stack<int>();
        foreach (var item in numberStack) { }
        #endregion

        #region Collections

        int[] nums = { 1, 2, 3 };

        LinkedList<int> ints = new LinkedList<int>();
        ArrayList arrayList = new ArrayList();
        List<string> list = new List<string>();
        Dictionary<string, int> dict = new Dictionary<string, int>();
        HashSet<int> set = new HashSet<int>();
        Queue<int> queue = new Queue<int>();
        Stack<int> stack = new Stack<int>();

        #endregion

        #region  LINQ

        var result = list.Where(x => x.Length > 3).ToList();
        var first = list.FirstOrDefault();
        var sorted = list.OrderBy(x => x).ToList();
        var grouped = list.GroupBy(x => x.Length);

        #endregion

        await GetDataAsync();

        await foreach (var n in GetNumbersAsync())
        {
            Console.WriteLine(n);
        }

        #region Input Handling (New & Old Systems)

        if (Input.GetKeyDown(KeyCode.Space)) { }
        if (Input.GetMouseButton(0)) { }

        /*public class PlayerInput : MonoBehaviour {
            public void OnJump(InputAction.CallbackContext ctx) {
                if (ctx.performed)
                    Debug.Log("Jump");
            }
        }*/

        #endregion

        #region Coroutines

        StartCoroutine(DelayedAction());

        IEnumerator DelayedAction()
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("Done");
        }

        #endregion

    }

    //Methods
    void SayHello(string name)
    {
        Console.WriteLine($"Hello, {name}");
    }

    int Add(int a, int b) => a + b;

    void OptionalParams(string name = "Guest") { }

    //Polymorphism 

    //Method Overloading (Compile-time Polymorphism)
    void Print(int x) { }
    void Print(string s) { }

    #region Exception Handling

    void devideANumber(int x, int y)
    {
        try
        {
            int result = x / y;
        }
        catch (DivideByZeroException ex)
        {
            Debug.Log($"Cannot divide by zero. {ex.Message}");
        }
        finally
        {
            Debug.Log("Always runs.");
        }

    }

    #endregion

    #region Async & Await

    async Task<int> GetDataAsync()
    {
        await Task.Delay(1000);
        return 42;
    }

    async IAsyncEnumerable<int> GetNumbersAsync()
    {
        for (int i = 0; i < 5; i++)
        {
            await Task.Delay(100);
            yield return i;
        }
    }

    #endregion

}


#region Method Overriding(Runtime Polymorphism)

public class Animal : MonoBehaviour
{
    public virtual void Speak()
    {
        Debug.Log("Animal speaks");
    }

    public virtual void Act()
    {
        Debug.Log("Animal does something");
    }

    public virtual bool IsWildAnimal()
    {
        return true;
    }
}

public class Dog : Animal
{
    public override void Speak()
    {
        Debug.Log("Dog barks");
    }

    public override void Act()
    {
        Debug.Log("Dog patrols with tail wagging.");
    }

    public override bool IsWildAnimal()
    {
        return false;
    }

    public void Fetch()
    {
        Debug.Log("Dog fetches ball!");
    }
}

public class Lion : Animal
{
    public override void Speak()
    {
        Debug.Log("Lion roars");
    }

    public override void Act()
    {
        Debug.Log("Lion stalks and pounces.");
    }

    public override bool IsWildAnimal()
    {
        return true;
    }
}

/*
   Why we use Animal a = new Dog(); here:
   We manage all agents using a single list: List<Animal>.
   
   Each AI agent has its own unique behavior (Act(), Speak()).
   
   We don’t care what kind of animal it is for general behavior.
   
   When we do need a specific behavior (Fetch()), we can cast safely.

*/

public class ZooManager : MonoBehaviour
{
    private List<Animal> animalAgents = new List<Animal>();

    void Start()
    {
        animalAgents.Add(new GameObject("DogAgent").AddComponent<Dog>());
        animalAgents.Add(new GameObject("LionAgent").AddComponent<Lion>());

        foreach (Animal agent in animalAgents)
        {
            Debug.Log($"== {agent.name} ==");
            agent.Speak();   // Automatically calls correct version
            agent.Act();     // Automatically calls correct version

            if (agent.IsWildAnimal())
                Debug.Log("This agent is wild.");
            else
                Debug.Log("This agent is domestic.");

            // agent.Fetch(); ❌ Not accessible here — because it's not in Animal
        }

        // Need Dog-specific behavior?
        Dog dog = animalAgents[0] as Dog;
        if (dog != null)
        {
            dog.Fetch(); // ✅ Specific behavior
        }
    }
}
#endregion

#region  Object-Oriented Programming

class Person : MonoBehaviour
{
    public string Name;
    public int Age;
    public int Id;

    public Person()
    {

    }

    public Person(int id)
    {
        Id = id;
    }

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public void Greet()
    {
        Debug.Log($"Hello, I’m {Name}");
    }

    private void Start()
    {
        var p = new Person ("Alice", 30 );
        var p1 = new Person {Name = "Alice", Age = 30 };
        var p2 = new Person ();
        var p3 = new Person (24);
        p.Greet();
    }

}

#endregion

#region Abstraction & Interfaces
abstract class Shape
{
    public abstract double Area();
}

interface IDrawable
{
    void Draw();
}
#endregion

#region Encapsulation (Properties)

class Car
{
    public string Model { get; set; }
    private int speed;

    public int Speed
    {
        get { return speed; }
        set
        {
            if (value > 0) speed = value;
        }
    }
}

#endregion

#region ScriptableObjects (Data-Driven Design)

[CreateAssetMenu(menuName = "Game/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string name;
    public int damage;
}

#endregion
