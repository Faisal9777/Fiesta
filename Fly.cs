using Rules;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : ComboableAbility<Basic>, ISpecialInteractable<Component>
{
    //private ISpecialInteractable<Component> interactor;
    //private IInteractor<Component> interactor;
    private IFrameByFrameStatusChecker statusChecker;
    private Basic passive;
    private Vector3 targetPosition;
    private float velocity;
    private float defaultGravity;
   
    private void ExecutePassive()//delete
    {
        if (Duration > 0)
        {
            Execute<Basic>("Execute");
            Duration -= 0.5f;

        }
        else
        {
            Execute<Basic>("Basic");
        }
        //Debug.Log("the character gravity " + Character.transform.GetComponent<Rigidbody2D>().gravityScale);
        //Debug.Log("the fly furation " + Duration);
    }
    private float duration = 0;
   
    public float Duration 
    {
        get => duration;
        set 
        {
            if(value <= 0)
            {
                duration = duration + value + (Character.GetStats("Dexterity") / 100f);
            }
            else
            {
                duration = value;
            }
        } }
    //public List<Del<Component>> ObjectRules { get; set; }
    public bool Activated { get; set; }//delete
    public Interactable<Component> Interactable { get; set; }

    protected override void InitializeIndependentVariables()
    {
        base.InitializeIndependentVariables();
        
        EnergyCost = 10f;
    }
    protected override void FillAnimation(Basic basic)
    {
        Float(Character, true);
        base.FillAnimation(basic);
    }

    public override void InitializeWeaponEquip(Character character)
    {
        var fly = FlyToPoint1(character);//delete
        Action action = () => { ActionPassive(fly); };//delete
        //passive = GetPassive(action);//delete
        base.InitializeWeaponEquip(character);


    }
    public virtual void InitializeWeaponEquip(Character character, AttackProcessor processor = null, Collider2D[] colliders = null)
    {
        base.InitializeWeaponEquip(character, processor);
        if (colliders != null)
        {
            this.ImplementInteractableInterfaceVariables(colliders);
        }
        else
        {
            this.ImplementInteractableInterfaceVariables(Character.GetComponents<Collider2D>());
        }
    }

    //public override void InitializeWeaponEquip(Character character, float duration)
    //{
      
    //    //var rb = character.transform.GetComponent<Rigidbody2D>();
    //    //defaultGravity = rb.gravityScale;
    //    //rb.gravityScale = 0;
    //    ////Debug.Log("the force acting on the body " +rb.velocity.magnitude);
    //    //rb.velocity -= rb.velocity;
    //    //rb.AddForce(-(rb.mass * rb.velocity.magnitude * Vector2.one));
    //    //character.transform.GetComponent<Rigidbody2D>().mass = 0;
    //    //Debug.Log("the character gravity InitializeWeaponEquip  " + character.transform.GetComponent<Rigidbody2D>().gravityScale);
    //    InitializeWeaponEquip(character);
    //    //InitializeWeaponEquip(duration);
        
    //}
    internal void InitializeWeaponEquip(float? duration = null)
    {
        
        Duration = duration ?? GetDuration();
        //targetPosition = Mouse.GetMousePosition2();
        velocity = Character.Agility * 0.2f / (Time.deltaTime);
        
    }
    public void InitializeWeaponEquip(Vector3? position)
    {

        //Duration = duration ?? GetDuration();
        targetPosition = position ?? Mouse.GetMousePosition2();
        //velocity = Character.Agility * 0.2f / (Time.deltaTime);

    }

    private float GetDuration()
    {
        return (Character.GetStats("Dexterity")*10f)/20f;
    }

    //internal void InitializeWeaponEquip(Character character, AttackProcessor processor, float? duration = null)
    //{
    //    InitializeWeaponEquip(character, processor);

    //    InitializeWeaponEquip(duration);
    //}
    internal void InitializeWeaponEquip(Character character, AttackProcessor processor = null, Collider2D[] colliders = null, float? duration = null)
    {
        InitializeWeaponEquip(character, processor, colliders);

        InitializeWeaponEquip(duration);
        
    }
    public override Func<ActionTask, IHandler> GetBasicAttack()
    {
        return Cancel;
    }

    //private Basic GetPassive(Task action)//delete
    //{
        
    //    AttackAnimation[] animations = new AttackAnimation[] { this.GetAnimation(IsValidKey, "Passive"), Character.GetAnimation(IsValidKey, "Fly_Passive") };
    //    return new Basic(action, animations);
    //}

    private void ActionPassive(Action<Transform, float> action)
    {
        //if (Duration != 0)
        //{
        //    action(character.transform, velocity);
        //}
        //else
        //{
        //    Cancel();
        //}
        //Duration -= EnergyCost / character.Dexterity;
        action(Character.transform, velocity);
    }

    private void ActionPassive(Action<Transform, float, Vector3> action)//delete
    {
        //if (Duration != 0)
        //{
        //    action(character.transform, velocity);
        //}
        //else
        //{
        //    Cancel();
        //}
        //Duration -= EnergyCost / character.Dexterity;
        action(Character.transform, velocity, targetPosition);
        //if (character.transform.position != targetPosition)
        //{
           
        //    action(character.transform, velocity, targetPosition);
        //}
        
    }

    //public void Cancel()
    //{

    //    //var rb = Character.transform.GetComponent<Rigidbody2D>();
    //    //rb.gravityScale = defaultGravity;
    //    //rb.velocity = Vector2.zero;
    //    Fly.Float(Character, false);
        
    //    Debug.Log("the default gravity " + defaultGravity);
    //    passive.StopAction();
    //    //passive = null;
    //    Dispose();
    //    //this.Dispose();
    //}
    //to be deleted
    //public void Cancel()
    //{

    //    //var rb = Character.transform.GetComponent<Rigidbody2D>();
    //    //rb.gravityScale = defaultGravity;
    //    //rb.velocity = Vector2.zero;
    //    Fly.Float(Character, false);

    //    Debug.Log("the default gravity " + defaultGravity);
    //    passive.StopAction();
    //    //passive = null;
    //    //Dispose();
    //    //this.Dispose();
    //}

    public IHandler Cancel(ActionTask task)
    {

        //var rb = Character.transform.GetComponent<Rigidbody2D>();
        //rb.gravityScale = defaultGravity;
        //rb.velocity = Vector2.zero;
        Fly.Float(Character, false);

        Debug.Log("the default gravity " + defaultGravity);
        return ActionTask.Completed(task);
        //passive.StopAction();
        //passive = null;
        //Dispose();
        //this.Dispose();
    }
   

    public static Action<Transform, float> FlyToPoint(Character transform)//delete
    {
        //Velocity += acceleration;
        transform.GetComponent<IMovable>().CanMove = false;
        //transform.LookAt(Mouse.GetMousePosition());
        return SpecialProperties.MoveToPoint;
    }

    public static Action<Transform, float, Vector3> FlyToPoint1(Character transform)//delete
    {
        //Velocity += acceleration;
        Debug.Log("flying to mouse point new FlyToPoinnt1");
        Fly.Float(transform, true);
        //transform.LookAt(Mouse.GetMousePosition());
        return SpecialProperties.MoveToPoint;
    }
    public IHandler FlyToPoint2(ActionTask task)
    {
        //Velocity += acceleration;
        Debug.Log("flying to mouse point new FlyToPoinnt2");
        Fly.Float(Character, true);
        return ActionTask.Completed(task);
        //transform.LookAt(Mouse.GetMousePosition());
        
        //return FlyToPoint3();
    }
    public Func<ActionTask, IHandler> FlyToPoint3()
    {

        Func<ActionTask, IHandler> action = (ActionTask task) => { SpecialProperties.MoveToPoint(Character.transform, velocity, targetPosition); return ActionTask.Completed(task); };
        return action;
    }

    public static void Float(Character obj, bool toggle)
    {

        //obj.CanMove = false;
        //obj.GetComponent<Rigidbody2D>().gravityScale = 0;
        if (Convert.ToBoolean(obj.GetComponent<Rigidbody2D>().gravityScale) == toggle)
        {
            obj.RemoveVelocity();
            obj.ToggleMovement(!toggle);
            obj.ToggleGravity(!toggle);
            Debug.Log("gravity of the character is " + obj.GetComponent<Rigidbody2D>().gravityScale);
        }
        
    }



    protected override string GetAttackType()
    {
        return "Basic";
    }

    //public override Del<ICalculator> GetObjectRule(Collider2D obj)
    //{
    //    throw new NotImplementedException();
    //}

    
#nullable enable
    protected override List<(string, float?)>? GetOtherStringEvents()
    {
        return null;
    }

    protected override List<(EventHandler, float?)>? GetOtherEvents()
    {
        return null;
    }

    public override (float?, float?) GetTriggerEventsTime()
    {
        return (0f, 0f);
    }


    public override void FiredActionEvent(object sender, EventArgs e)//may not be required
    {
        var input = sender.ToString();
        if (input == "Execute")
        {
            //AttackHolder.Processor.c_PerformNextAction(null, null);
        }
    }

    public override string GetAttackLabel()
    {
        return "Basic";
    }

    public override Func<ActionTask, IHandler> GetComboAttack(int index)
    {
        switch (index)
        {
            case 1:
                return FlyToPoint2;
            case 2:
                return FlyToPoint3();
            case 3:
                return GetBasicAttack();


        }
        throw new Exception("Task index out of bound");
        
    }

    public override string GetAttackTypes(int index)
    {
        return "Basic";
    }

    public override int[] GetAttackSuccessors(int index)
    {
        switch (index)
        {
            case 1:
                return new int[] { 2 };
            case 2:
                return new int[] { 3 };
            case 3:
                return new int[] { 1 };


        }
        throw new Exception("Task index out of bound");
    }

    public override List<(string, float?)>? GetStringEventsForWeapon(int index)
    {
        return null;
    }

    public override List<(EventHandler, float?)>? GetEventsForWeapon(int index)
    {
        return null;
    }

    public override (float?, float?) GetTriggerEventsTime(int actionIndex)
    {
        switch (actionIndex)
        {
            case 1:
                return GetTriggerEventsTime();
            case 2:
                return (null, null);
            case 3:
                return GetTriggerEventsTime();


        }
        throw new Exception("Task index out of bound");
    }

    public override string GetAttackLabel(int index)
    {
        switch (index)
        {
            case 1:
                return GetAttackLabel();
            case 2:
                return "";
            case 3:
                return "Cancel";


        }
        throw new Exception("Task index out of bound");
    }

   
    public override float? GetActionLength(int index)//action length not used
    {
        if(index == 3)
        {
            return GetActionLength();
        }
        return null;
    }

    public override float? GetActionLength()//action length not used
    {
        return 0f;
    }

    public override int GetComboLength(int? length = 0)
    {
        return 3;
    }

    public void Handle(Component player)
    {
        Interactable.Handle(player);
    }

    //public void InitializeInteractor<T1>() where T1 : Component
    //{
    //    interactor = new Interactor<Component>();
    //}

   

 


    public void InitializeStatusChecker()//delete
    {
        statusChecker = new ObjectSimulation<Component>(this.transform);
    }

    void ISpecialInteractable<Component>.GetObjectRule(Component obj)
    {
        AttackHolder.Execute<Basic>("Cancel");
    }

    public string GetTag()
    {
        return tag;
    }

    public Collider2D[] GetColliders()//delete
    {
        return Character.GetComponents<Collider2D>();
    }

    //public IInteractor<Component> GetIntercator()
    //{
    //    throw new NotImplementedException();
    //}

    //public void InitializeStatusChecker(Collider2D[] colliders)
    //{
    //    throw new NotImplementedException();
    //}

    public Interactable<Component> GetInitializedInteractable(Collider2D[] colliders, string name)
    {
        return new Interactable<Component>(new ObjectSimulation<Component>(colliders), new Interactor<Component>(), name);
    }


    protected override void FiestaFixedUpdate()
    {
        Interactable.Simulation.CheckStatus();
        AttackHolder.Execute<Basic>("Basic");
    }

    protected override void FiestaUpdate()
    {
        
        base.FiestaUpdate();
    }

    public void ProcessCollisionalResult(string collisionalType, float amount)
    {
        
    }

    public override bool RunActionByFrame(int index)
    {
        if(index == 2)
        {
            return true;
        }
        return false;
    }
#nullable enable
    public override ICondition[]? GetAllowingPushConditions(int index)
    {
        if(index == 2)
        {
            Func<bool> condition = () => { return false; };
            return new ICondition[] { new Condition(condition) };
        }
        return null;
    }

    public override ICondition[]? GetEndConditions(int index)
    {
        if (index == 2)
        {
            Func<bool> func = () => { if (Duration == 0) { return true; } return false; };
            var condition = new Condition(func);
            ICondition[] conditions = new[] { condition };
            return conditions;
        }
        return null;
    }

    protected override IAttackHolder<Basic> GetInitializedAttackHolder(AttackProcessor attackProcessor)
    {
        return new ComboActions<Basic>(attackProcessor);
    }

    public override ICondition[]? GetAllowingPushConditions()
    {
        return null;
    }

    public override ICondition[]? GetEndConditions()
    {
        return null;
    }

    public override ICondition[] GetConditionToExecuteAction()
    {
        Predicate<float> predicate = (float time) => { return time == 0f; };
        ICondition[] conditions = new Condition[] { new Condition<float>(predicate) };
        return conditions;
    }

    public override ICondition[] GetConditionToExecuteAction(int index)
    {
        switch (index)
        {
            case 1:
                return GetConditionToExecuteAction();
            case 2:
                return GetConditionToExecuteAction();
        }
        throw new Exception("Task index out of bound");
    }

    public override bool IsResettable(int i)
    {
        switch (i)
        {
            case 1:
                return true;
            case 2:
                return true;
            case 3:
                return false;


        }
        throw new Exception("Task index out of bound");
    }


    //protected override Basic 
}

public class ReplaceFly : ReplaceWeapon<Basic>, ISpecialInteractable<Component>
{
    public bool Activated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void GetObjectRule(Component obj)
    {
        throw new NotImplementedException();
    }

    public string GetTag()
    {
        throw new NotImplementedException();
    }

    public void Handle(Component player)
    {
        throw new NotImplementedException();
    }

    public void OnNext(Component obj)
    {
        throw new NotImplementedException();
    }

    public static void FlyToPoint(IMovable obj, float velocity, Vector3 targetPoint)
    {
        //Debug.Log("Im moving to the point with x " + targetPoint.x + " and y " + targetPoint.y);
        //Debug.Log("Im currently at point with x " + obj.position.x + " and y " + obj.position.y);
        var distanceVector = targetPoint - obj.Transform.position;
        if (distanceVector.magnitude > velocity * Time.fixedDeltaTime)
        {
            CharacterController2D.MoveTo(obj, distanceVector);
            obj.position += Time.fixedDeltaTime * velocity * distanceVector.normalized;
        }
        else
        {
            Debug.Log("i have reached my destination");
            obj.position = targetPoint;
            Debug.Log("my position right now is " + obj.position);
        }
        //obj.Translate(0.0f, 0.0f, velocity * Time.deltaTime);
    }
}