using System;
using System.Collections.Generic;
using UnityEngine;

public class Basic : IAction
{
    public event EventHandler RegisterAttack;
    public event EventHandler ExecuteNextAction;
    public event EventHandler ActionEvent;
    private float timeToExecuteAction;
    private List<(EventHandler, float?)> events;
    public Type TriggerType { get; set; }
    public float ActionLength { get; set; }
    public ActionTask Task { get; internal set; }
    private FiestaTime _duration;
#nullable enable
    protected object? pushableTime;
    protected object? attackEndTime;
    protected ICondition[]? pushableConditions;
    protected ICondition[]? endConditions;
    protected ICondition[] attackExecuteCondition;
#nullable disable
    public AttackAnimation[] Animations { get; internal set; }

    //public bool IsActivated { get; internal set; }
    public bool IsProcessing { get; internal set; }
    public bool ActionableByFrame { get; set; }
    public bool IsProcessedInFixedUpdate { get; set; }
    //public bool IsParallelable { get; set; }
    public float Duration
    {
        get
        {
            return _duration.GetTimeInSeconds();
        }
        set
        {
            
            if(events != null)
            {
                _duration.SetTimeInSeconds(value);
                
                
            }
        }
    }

    public bool IsBasicCompleted { get; set; }
    public bool IsPushAllowed { get; set; }

    private void RunEvent()
    {
        
        
        while (events[0].Item2 != Duration)
        {
            events[0].Item1.Invoke(null, null);
            events.RemoveAt(0);
        }


    }

    //private List<IActionEvent> events;


    
    //public AttackProcessor Processor { get; set; }
    //private bool CanRegisterNextAttack;

    //public bool IsIdle { get; internal set; }

    //public Basic(Task attack, AttackAnimation?[] animations, (string, float?) nextAttackFrame, (string, float?) animationEndFrame) : this(attack, animations)
    //{
    //    //this(attack);
    //    if (animations[0] != null)
    //    {
    //        AddAnimation(animations, nextAttackFrame, animationEndFrame);
    //    }




    //    //IsActivated = false;
    //}

    //public Basic(Task attack, AttackAnimation?[] animations, int[] successors) : this(attack, animations)
    //{


    //    Successors = successors;


    //    //IsActivated = false;
    //}

    //public Basic(Task attack, AttackAnimation?[] animations) : this(attack)
    //{
    //    //this(attack);
    //    if (animations[0] != null)
    //    {
    //        AddAnimation(animations);
    //    }




    //    //IsActivated = false;
    //}
    //public Basic(Task attack, AttackAnimation?[] animations, string attackLabel) : this(attack, animations)
    //{
    //    //this(attack);
    //    AttackLabel = attackLabel;




    //    //IsActivated = false;
    //}
    //public Basic(Task attack, AttackAnimation?[] animations, string attackLabel, float? actionLabel) : this(attack, animations, attackLabel)
    //{
    //    //this(attack);
    //    ActionLength = actionLabel ?? 0f;




    //    //IsActivated = false;
    //}
#nullable enable
    //public Basic(Task attack, AttackAnimation?[] animations, string attackLabel, float? actionLabel, float? pushableTime, float? attackEndTime) 
    //{
    //    SetBasic(attack, animations, attackLabel, actionLabel, pushableTime, attackEndTime);


    //    //ActionLength = actionLabel ?? 0f;
    //    //AttackLabel = attackLabel;
    //    //if (animations[0] != null)
    //    //{
    //    //    AddAnimation(animations!);
    //    //}
    //    //Task += attack;
    //    //SetTrigger();
    //    //this.pushableTime = pushableTime;
    //    //this.attackEndTime = attackEndTime;
    //    //pushableConditions = null;
    //    //endConditions = null;
        


    //    //IsActivated = false;
    //}
    public void SetBasic(Func<ActionTask, IHandler> attack, AttackAnimation?[] animations, float? actionLabel, object? pushableTime, object? attackEndTime)
    {
        

        ActionLength = actionLabel ?? 0f;   
        if (animations[0] != null)
        {
            AddAnimation(animations!);
        }
        ActionTask task = new(attack);
        Task = task;
        SetTrigger();
        this.pushableTime = pushableTime;
        this.attackEndTime = attackEndTime;
        //pushableConditions = null;
        //endConditions = null;



        //IsActivated = false;
    }
    public Basic(Func<ActionTask, IHandler> attack, ICondition[] attackExecuteCondition, AttackAnimation?[] animations, float? actionLength, object? pushableTime, object? attackEndTime, bool isUpdatedInFixed)
    {


        SetBasic(attack, animations, actionLength, pushableTime, attackEndTime);
        this.attackExecuteCondition = attackExecuteCondition;
        this.IsProcessedInFixedUpdate = isUpdatedInFixed;



        //IsActivated = false;
    }
    public Basic(Func<ActionTask, IHandler> attack, ICondition[] attackExecuteCondition, AttackAnimation?[] animations, float? actionLength, object? pushableTime, object? attackEndTime, ICondition[]? pushableConditions, ICondition[]? endConditions, bool isUpdatedInFixed) : this(attack, attackExecuteCondition, animations, actionLength, (object?)pushableTime, attackEndTime, isUpdatedInFixed)
    {
        
        this.pushableConditions = pushableConditions!;
        this.endConditions = endConditions!;
        






        //IsActivated = false;
    }


    //protected Basic(object? pushableTime, object? attackEndTime) 
    //{
    //    //this(attack);
    //    this.pushableTime = pushableTime;
    //    this.attackEndTime = attackEndTime;





    //    //IsActivated = false;
    //}

    public void InsertEvent(EventHandler handler, float time)
    {
        (EventHandler, float) evt = (handler, time);
        events.Add(evt);
    }
    public virtual bool IsPushable()
    {
        try
        {
            if(pushableTime == null && pushableConditions!.Length < 1)
            {
                return true;
            }
            else
            {
                return (Equals(Duration, pushableTime) || (float?)Duration > (float?)pushableTime) && ConditionMet(pushableConditions);

            }
            

        }
        catch (Exception)
        {

            return false;
        }
        
        //return ValidateParameter(type, pushableTime);
    }
    protected virtual bool IsAttackCompleted()
    {
        try
        {
            if(attackEndTime == null && endConditions!.Length < 1)
            {
                return true;
            }
            else
            {
                return IsBasicCompleted;
            }
            
        }
        catch (Exception)
        {

            return false;
        }
        
    }
    protected virtual bool IsPushable(object pushableParameter)
    {
        try
        {
            return CheckParameter(pushableConditions, pushableParameter) || Equals(pushableParameter, pushableConditions);


        }
        catch (Exception)
        {

            return false;
        }

        //return ValidateParameter(type, pushableTime);
    }
    protected virtual bool IsAttackCompleted(object endParameter)
    {
        try
        {
            return (CheckParameter(endConditions, endParameter) || Equals(endParameter, attackEndTime));

        }
        catch (Exception)
        {

            return false;
        }

    }
    public static bool CheckParameter<L>(ICondition[]? conditions, L conditionalParameter)
    {
        if(conditions != null && conditions.Length > 0)
        {
            foreach (var item in conditions)
            {
                if (item.GetType().IsAssignableFrom(typeof(Condition<L>)))
                {
                    if (conditionalParameter != null)
                    {
                        if (!(((Condition<L>)item).ConditionMet(conditionalParameter)))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
                else if (!item.ConditionMet())
                {
                    return false;
                }
            }
        }
        

        return true;
    }

    public static bool ConditionMet(ICondition[]? conditions)
    {
        if(conditions !=  null && conditions.Length > 0)
        {
            foreach (var item in conditions)
            {
                if (item != null && !item.ConditionMet())
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void FireEvent(string input)
    {
        ActionEvent?.Invoke(input, null);
    }
    //public Basic(Task attack, AttackAnimation?[] animations, (string, float?) nextAttackFrame, (string, float?) animationEndFrame, List<(string, float?)> events)
    //{

    //}
    //public Basic(Task attack, AttackAnimation?[] animations, (string, float?) nextAttackFrame, (string, float?) animationEndFrame, List<(Task, float?)> events)
    //{

    //}

    //public static Basic CreateAttack(Task attack, AttackAnimation?[] animations, float? nextAttackFrame, float? animationEndFrame)
    //{
    //    if(animations[0] == null)
    //    {
    //        return new Basic(attack);
    //    }
    //    float atkFrame = nextAttackFrame ?? default(float);
    //    float animEndFrame = animationEndFrame ?? default(float);
    //    return new Basic(attack, animations!, atkFrame, animEndFrame);
    //}
#nullable disable
    //protected Basic(Task attack) 
    //{
    //    Task += attack;
    //    SetTrigger();
    //}
#nullable enable
    protected virtual void SetTrigger()
    {
        TriggerType = typeof(Basic);
    }

    //public Basic()
    //{
    //    //IsActivated = false;
    //}
    
    public void AddAnimation(AttackAnimation?[] animations, (string, float?) nextAtkFrame, (string, float?) animationEndFrame)
    {
        if(animations != null)
        {
            Animations = new AttackAnimation[animations.Length];
            for (int i = 0; i < animations.Length; i++)
            {
                try
                {
                    if (animations[i] != null)
                    {
                        Animations[i] = animations[i];
                        AddEvent(Animations[i], nextAtkFrame, animationEndFrame);
                        return;
                    }


                }
                catch
                {
                    Animations[i] = null;
                }


            }
        }
        else
        {
            Animations = null;
        }
        


    }

    public virtual void Update(object? param = null)
    {
        if (!IsBasicCompleted)
        {
            CheckIfAttackCompleted(param);
            CheckIfPushable(param);

            
        }
        
        

    }

    private void CheckIfAttackCompleted(object? param = null)
    {
        try
        {
            if(param == null)
            {
                if (attackEndTime != null)
                {
                    IsBasicCompleted = Equals(_duration, attackEndTime) || (float?)Duration > (float?)attackEndTime;
                }
                else
                {
                    IsBasicCompleted = ConditionMet(endConditions);
                }
            }
            else
            {
                IsBasicCompleted = IsAttackCompleted(param);
            }
            
            

        }
        catch (Exception)
        {
            IsBasicCompleted = false;

        }
    }

    private void CheckIfPushable(object? param = null)
    {
        try
        {
            if (param == null)
            {
                IsPushAllowed = IsPushable();
            }
            else
            {
                IsPushAllowed = IsPushable(param);
            }



        }
        catch (Exception)
        {


        }
    }

    protected void ExecuteAction()
    {
        Task.Run();
        //isActionCompleted = true;

    }

    public void AddAnimation(AttackAnimation?[] animations)
    {
        if (animations != null)
        {
            Animations = new AttackAnimation[animations.Length];
            for (int i = 0; i < animations.Length; i++)
            {
                try
                {
                    if (animations[i] != null)
                    {
                        Animations[i] = animations[i];
                        //AddEvent(Animations[i], nextAtkFrame, animationEndFrame);
                        return;
                    }


                }
                catch
                {
                    Animations[i] = null;
                }


            }
        }
        else
        {
            Animations = null;
        }



    }

    protected virtual void AddEvent(AttackAnimation animation, (string, float?) nextAtkFrame, (string, float?) animationEndFrame)
    {
        //AnimationClip clip = animation.Clip;
        //float aniTime = 0f;
        if(nextAtkFrame.Item2 > 0)
        {
            AddEventToClip(nextAtkFrame.Item2 ?? default, nextAtkFrame.Item1, animation!.Clip);
        }
        if(animationEndFrame.Item2 > 0)
        {
            AddEventToClip(animationEndFrame.Item2 ?? default, nextAtkFrame.Item1, animation!.Clip);
        }

        //AddEvent(aniTime*0.5f, "CanNextAttack", animation.Clip);
        //AddEvent(animation.Clip.length, "PerformNextAction", animation.Clip);
        //AddEvent(aniTime * 0.6f, "CanNextAttack", animation.Clip);




    }

    protected virtual void AddEvent(List<(string, float?)> events, int animationsIndex)
    {

        for (int i = 0; i < events.Count; i++)
        {
            if (events[i].Item2 != null && events[i].Item1 != null)
            {
                AddEventToClip(events[i].Item2 ?? default, events[i].Item1, Animations[animationsIndex].Clip);
            }
            
            //AddEventToClip(weaponEvents[i].Item2, weaponEvents[i].Item1, Animations[1].Clip);
        }

    }
    

    public virtual void AddEvent(List<(string, float?)>? characterEvents, List<(string, float?)>? weaponEvents)
    {
        int remaining;
        if(characterEvents != null && weaponEvents != null)
        {
            if (characterEvents.Count > weaponEvents.Count)
            {
                remaining = characterEvents.Count - weaponEvents.Count;
                AddEvent(characterEvents, weaponEvents, remaining, 0, 1);
            }
            else if (weaponEvents.Count > characterEvents.Count)
            {
                remaining = weaponEvents.Count - characterEvents.Count;
                AddEvent(weaponEvents, characterEvents, remaining, 1, 0);
            }
            else
            {
                for (int i = 0; i < characterEvents.Count; i++)
                {
                    if (characterEvents[i].Item2 != null)
                    {
                        AddEventToClip(characterEvents[i].Item2 ?? default, characterEvents[i].Item1, Animations[0].Clip);
                        AddEventToClip(weaponEvents[i].Item2 ?? default, weaponEvents[i].Item1, Animations[1].Clip);
                    }

                }
            }
        }
        

    }
    //not being used for now
    private void AddEvent(List<(string, float?)> longEventList, List<(string, float?)> shortEventList, int remaining, int animationsIndexForLongList, int animationsIndexForShortList)
    {
        for (int i = 0; i < shortEventList.Count; i++)
        {
            AddEventToClip(longEventList[i].Item2 ?? default, longEventList[i].Item1, Animations[animationsIndexForLongList].Clip);
            AddEventToClip(shortEventList[i].Item2 ?? default, shortEventList[i].Item1, Animations[animationsIndexForShortList].Clip);
        }
        for (int i = longEventList.Count - remaining; i < longEventList.Count; i++)
        {
            AddEventToClip(longEventList[i].Item2 ?? default, longEventList[i].Item1, Animations[animationsIndexForLongList].Clip);
        }

    }
    public virtual void AddEvent(List<(EventHandler, float?)>? events)
    {
        if(events != null && events[0].Item1 != null && events[0].Item2 != null)
        {
            if (this.events.Count == 0)
            {
                this.events = events;
            }
            else
            {
                this.events.AddRange(events);
            }

        }
        
        

    }
    public virtual void AddEvent(List<(string, float?)>? events)
    {

        if (events != null)
        {
            AddEvent(events, 1);

        }
        


    }

    protected void AddEventToClip(float time, string functionName, AnimationClip clip)
    {
        
        AnimationEvent evt2 = new()
        {

            time = time,
            functionName = functionName
        };
        clip.AddEvent(evt2);
    }

    public virtual void Execute()
    {
        IsProcessing = true;
        //Debug.Log("Playing action");
        Task.Run();
        if (events != null)
        {
           
            RunEvent();

        }
        else
        {
            try
            {
                foreach (var item in Animations)
                {
                    //Debug.Log("Playing animation");
                    //Debug.Log("Playing animation now " + item.Clip.name);
                    var stuff = UnityEditor.AnimationUtility.GetAnimationEvents(item?.Clip);
                    //Debug.Log("number of events of the clip " + item.Clip.name + " are " + stuff.Length + " with the first function time " + stuff[0].time + " with 2nd funnction time " + stuff[1].time);
                    item?.Play();
                }

            }
            catch
            {
                //Debug.Log("error");
            }
        }
        //Debug.Log("action played");
        
        



        //IsActivated = false;
    }

    public void Activate()
    {
        //IsActivated = true;
    }

    internal static bool IsAnyAttackType(string v)
    {
        throw new NotImplementedException();
    }

    //protected virtual void CanNextAttack(EventArgs e)
    //{
    //    NextAttack?.Invoke(this, e);
    //}




    internal void CanNextAttacks()
    {
        RegisterAttack.Invoke(null, null);
    }

    internal virtual void PerformNextActions()
    {
        StopAction();
        //IsProcessing = false;
        ExecuteNextAction?.Invoke(this, null);
    }

    public void StopAction()
    {
        if(Animations != null)
        {
            foreach (var item in Animations)
            {
                if (item != null)
                {
                    item.Stop();
                }

            }
        }
        Task.StopTask();
        
    } 

    

    public Basic GetAttack()
    {
        return this;
    }

    //public void Execute<G>() where G : Basic
    //{
    //    if(Processor.Processing != null)
    //    {
    //        if (Processor.Processing.CanRegisterAttack())
    //        {
    //            var attack = GetAttack();
    //            if (attack.GetType() == typeof(G))
    //            {
    //                Processor.Push(attack);
    //            }
    //        }
    //    }
        
    //}

    

    
    public virtual bool CanRegisterAttack()
    {
        throw new NotImplementedException();
        //return CanRegisterNextAttack;
    }

    internal void AddEventForCharacter(List<(string, float?)>? events)
    {
        if (events != null)
        {
            AddEvent(events, 0);
        }
        
    }

    internal void AddEventForCharacter(List<(EventHandler, float?)>? events)
    {
        if (events != null)
        {
            AddEvent(events);
        }

    }

    public virtual void AdvanceTime(float time)
    {
        //if (!IsBasicCompleted)
        //{
        //    CheckIfAttackCompleted();
        //    CheckIfPushable();

        //    if (ActionableByFrame || CheckParameter(attackExecuteCondition, Duration))
        //    {
        //        ExecuteAction();
        //    }
        //    if (events != null)
        //    {
        //        _duration.UpdateTime(time);
        //        //if (ActionLength > 0f)
        //        //{
        //        //    _duration += Time.deltaTime;
        //        //}
        //        RunEvent();
        //    }
        //}

        CheckIfAttackCompleted();
        CheckIfPushable();

        if (ActionableByFrame || CheckParameter(attackExecuteCondition, Duration))
        {
            ExecuteAction();
        }
        if (events != null)
        {
            //if (ActionLength > 0f)
            //{
            //    _duration += Time.deltaTime;
            //}
            RunEvent();
        }
        Duration += time;
    }

    //public virtual void Push(Basic attack)
    //{
    //    this.Task = attack.Task;
    //    this.Animations = attack.Animations;
    //}
}

public class ActionTask : IObserver<Func<ActionTask, IHandler>>
{
    public bool IsActionCompleted { get; set; } = false;
    private Func<ActionTask, IHandler> task;
    protected IHandler? handler;
    public ActionTask(Func<ActionTask, IHandler> func)
    {
        task = func;
        handler = null;
    }

    public void OnCompleted()
    {
        IsActionCompleted = true;
        handler = null;
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(Func<ActionTask, IHandler> value)
    {
        throw new NotImplementedException();
    }

    public void Run()
    {
        if(handler == null)
        {
            handler = task(this);
        }
        
    }
    public void StopTask()
    {
        if (handler != null)
        {
            handler.End();
        }

    }
#nullable disable
    internal static IHandler Completed(ActionTask task)
    {
        task.OnCompleted();
        return null;
    }
}

public interface IAction
{
#nullable enable
    public bool IsProcessedInFixedUpdate { get; set; }
    public void Execute();
    public void StopAction();
    public bool IsBasicCompleted { get; set; }
    public bool IsPushAllowed { get; set; }
    public void AdvanceTime(float time);
    public void Update(object? param = null);
}

public class Basic<T> : Basic
{
    public Basic(Action)
    {

    }
}
