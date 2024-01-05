using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AttackProcessor : Processor, ITimeAffected
{
    //private float? attackRegisterableTime;
   // private float? attackEndTime;
    public bool IsPushable { get; set; }
    //private bool isProcessing = false;
    //public (T, float?, float?) attack;
    
    private event EventHandler OnActionFinished;
    //private ICondition[] processingPushConditions;
    private ICondition[] processingEndConditions;
    //private ICondition[] queuePushConditions;
   //private ICondition[] queueEndConditions;
   // public Basic NextInQueue { get; internal set; }
    private Queue waitingQueue;//will get deleeted
    private ProcessorQueue queue;
    private bool priority;

    public event EventHandler GoingIdle;
    

    


    public AttackProcessor(TaskAdvancer taskAdvancer) : base(taskAdvancer)
    {
        //this.taskAdvancer = taskAdvancer;
        IsPushable = true;
        Processing = null;
    }

    public void c_ReadyNextAttack(object sender, EventArgs e)//may get deleted
    {
        Debug.Log("can push nexxt attack");
        //if(Processing != null)
        //{
        //    IsPushable = true;
        //}
        IsPushable = true;

    }

    public void c_PerformNextAction(object sender, EventArgs e)//may get deleted
    {
        Debug.Log("performing next action");
        Processing.StopAction();
        //UnRegisterEvent();
        Processing = null;
        //IsPushable = false;
        if (waitingQueue != null)
        {
            //var temp = attack;
            //attack = null;
            //IsPushable = true;
            Debug.Log("processing next action");
            //Process();
        }
        else
        {
            Debug.Log("going idle");
            //Processing = null;
            Reset();
            GoingIdle?.Invoke(null, null);//may get deleted
        }
        

    }

    private void UnRegisterEvent()
    {
        Debug.Log("Unregistering events");
        Processing.RegisterAttack -= c_ReadyNextAttack;
        Processing.ExecuteNextAction -= c_PerformNextAction;
    }





    //public bool Push(Basic attack, ICondition[] allowPushconditions, ICondition[] endConditions, bool priority = false) //the method pushes attack to the processing depending on who has the higher priority among the pushed action and the action already in queue waiting
    //{
    //    //Debug.Log("pushing nexxt attack " + attack.Animations[0].Clip.name);
    //    if (IsPushable || priority == true)
    //    {

    //        //IsPushable = false;
    //        //try
    //        //{
    //        //    if (Processing != null && (Processing as Hold).HoldTrigger == typeof(Hold))
    //        //    {
    //        //        (Processing as Hold).Finish();
    //        //    }
    //        //}
    //        //catch { }

    //        //Debug.Log("pushing nexxt attack " + attack.Animations[0].Clip.name);
    //        //if (Processing == null)
    //        //{

    //        //    Process(attack);

    //        //}
    //        //else
    //        //{
    //        //    if(this.attack == null)
    //        //    {
    //        //        Debug.Log("attack is null");
    //        //        this.attack = attack;

    //        //    }
    //        //    if(Processing.AttackType == typeof(Hold))
    //        //    {
    //        //        ((Hold)Processing).Finish();
    //        //    }

    //        //}
    //        if (NextInQueue == null)
    //        {
    //            NextInQueue = attack;
    //            //this.priority = priority;
    //            queuePushConditions = allowPushconditions;
    //            queueEndConditions = endConditions;
    //            IsPushable = false;
    //            Process();
    //            return true;
    //        }
    //        else
    //        {
    //            if(priority == true && priority != this.priority)
    //            {

    //                Process(attack, allowPushconditions, endConditions);
    //                return true;
    //            }
    //            else
    //            {

    //                PerformNextAction();
    //                Push(attack, allowPushconditions, endConditions, priority);
    //                return true;

    //            }
    //        }
    //        //return false;

    //        //if (Processing.AttackType == typeof(Hold))
    //        //{
    //        //    ((Hold)Processing).Finish();
    //        //}
    //    }
    //    return false;




    //}
    //the following method may get deleted later
    public bool PushWithoutStartingProcessing(Basic attack, bool priority = false, EventHandler evt = null, EventHandler evt2 = null) //the method pushes attack to the processing depending on who has the higher priority among the pushed action and the action already in queue waiting
    {
        //Debug.Log("pushing nexxt attack " + attack.Animations[0].Clip.name);
        if (IsPushable || priority == true)
        {
            if (waitingQueue == null)
            {
                waitingQueue = new(attack, evt, evt2);
                IsPushable = false;
                return true;
            }
            //return false;
            
        }
        return false;




    }
    //the following method may get deleted later
    public bool Push(Basic attack, bool priority = false, EventHandler evt = null, EventHandler evt2 = null) //the method pushes attack to the processing depending on who has the higher priority among the pushed action and the action already in queue waiting
    {
        //Debug.Log("pushing nexxt attack " + attack.Animations[0].Clip.name);
        //if (IsPushable || priority == true)
        //{
        //    if (waitingQueue == null)
        //    {
        //        waitingQueue = new(attack, allowPushconditions, endConditions, evt);
        //        IsPushable = false;
        //        Process();
        //        return true;
        //    }
        //    else
        //    {
        //        if (priority == true && priority != this.priority)
        //        {

        //            evt?.Invoke(attack, null);
        //            Process(attack, allowPushconditions, endConditions);
        //            return true;
        //        }
        //        else
        //        {

        //            PerformNextAction();
        //            Push(attack, allowPushconditions, endConditions, priority, evt);
        //            return true;

        //        }
        //    }
        //    //return false;

        //    //if (Processing.AttackType == typeof(Hold))
        //    //{
        //    //    ((Hold)Processing).Finish();
        //    //}
        //}
        //return false;

        if (PushWithoutStartingProcessing(attack, priority, evt, evt2))
        {
            //Process();
            return true;
        }
        else if(priority == true)
        {
            if (priority != this.priority)
            {

                evt?.Invoke(attack, null);
                SetProcessedEvent(evt2);
                Process(attack);
                //return true;
            }
            else
            {

                PerformNextAction();
                Push(attack, priority, evt);
                //return true;

            }
            return true;
        }
        //else if (IsPushable = Processing.IsPushable(attack))
        //{
        //    Push(attack, priority, evt, evt2);
        //}
        return false;


        

    }
    //the following method may get deleted later
    public bool PushWithEnd(Basic attack, bool priority = false, EventHandler evt = null, EventHandler evt2 = null) //the method pushes attack to the processing depending on who has the higher priority among the pushed action and the action already in queue waiting
    {
        if (Processing != null)
        {
          //  Processing.IsAttackCompleted(attack);
        }

        return Push(attack, priority, evt, evt2);
    }

    //internal void ValidatePushability<P>(P conditionalParameter)
    //{
    //    IsPushable = Basic.CheckParameter(processingPushConditions, conditionalParameter);
    //}

    
    //the following may get replaced if parameter push gets replaced
    public bool Push<T, L>(Basic attack, L endTime, bool priority = false, EventHandler evt = null, EventHandler evt2 = null) //the method pushes attack to the processing depending on who has the higher priority among the action thats about to get pushed and the action already in queue waiting
    {

        if (attack == Processing)
        {
            //return PushCheckingPushability(attack, allowPushconditions, endConditions, endTime, priority);
            throw new Exception("The attack being pushed cannot be same when button is released");
            //return false;
        }
        else
        {
            if (((Hold<T, L>)Processing).IsAttackCompleted(endTime))
            {
                FinishCurrentTask();
                PerformNextAction();
            }
            else
            {
                return false;
            }
            //ValidatePushability(endTime);
            return Push(attack, priority, evt, evt2);
        }
        //else
        //{
        //    return Push(attack, allowPushconditions, endConditions, priority);
        //}
       

    }
#nullable enable
    internal IHandler[]? Push<G>(Basic action, bool priority, IActionObserver[]? observers)
    {
        if (typeof(G).IsAssignableFrom(typeof(Hold)) && action == Processing)
        {

            ((Hold)Processing).HoldThisFrame();
            //ValidatePushability(pushability);
            //Processing.Update();
            return null;

        }
        else
        {
            return Push(action, priority, observers); //if G is not hold then call Push without generic
        }
    }
    internal IHandler[]? ForcePush<G>(Basic action, bool priority, IActionObserver[]? observers)
    {
        if (Processing != null)
        {
            FinishCurrentTask();
            AllowPush();
            
        }
        return Push<G>(action, priority, observers);

    }

    public IHandler[]? Push(Basic action, bool priority, IActionObserver[]? observers)
    {
        IHandler[]? handlers;

        if ((handlers = PushWithoutStartingProcessing(action, observers)) != null)
        {
            //ProcessActionInQueue();
            if(Processing == null)
            {
                ProcessAction(queue.NextInQueue, queue.Observers.ToList());
            }
            return handlers;
        }
        else if (priority == true)
        {
            if (priority != this.priority)
            {
                return ForcePush(action, observers);
                //return GetHandlers(observers);
                //return true;
            }
            else
            {

                PerformNextAction();
                return Push(action, priority, observers);
                //return true;

            }
            
        }
        //else if (IsPushable = Processing.IsPushable(attack))
        //{
        //    Push(attack, priority, evt, evt2);
        //}
        return null;
    }
    internal IHandler[]? PushWithoutStartingProcessing(Basic action, IActionObserver[]? observers)
    {
        if (IsPushable)
        {
            if (queue == null)
            {
                queue = new(action, observers);
                IsPushable = false;
                return GetHandlers(observers);
                
                

                
            }
            //return false;

        }
        return null;
    }

    protected override void OnActionFinised()
    {
        base.OnActionFinised();
        PerformNextAction();
    }

    

    

    //the following may get replaced if parameter push gets replaced

    public bool PushCheckingPushability<T, L>(Basic attack, T pushability, bool priority = false, EventHandler? evt = null, EventHandler? evt2 = null) //the method pushes attack to the processing depending on who has the higher priority among the pushed action and the action already in queue waiting
    {

        if (attack == Processing)
        {

            IsPushable = ((Hold<T, L>)Processing).IsPushable(pushability);
            //ValidatePushability(pushability);
            //Processing.Update();
            return false;
           
        }
        else
        {
            return Push(attack, priority, evt, evt2);
        }

    }
    //public bool Push(T attack, (float?, float?) attackDetails)
    //{
    //    attackRegisterableTime = attackDetails.Item1;
    //    attackEndTime = attackDetails.Item2;
    //    return Push(attack);



    //}

    public void NextFrame() //will get deleted as replaced by AdvanceTime() from Processor Class
    {
        if (Processing != null)
        {
            //if(Processing.Duration == attackRegisterableTime)
            //{
            //    c_ReadyNextAttack(null, null);
            //}
            //if(Processing.Duration == attackEndTime)
            //{
            //    c_PerformNextAction(null, null);
            //}
            //else
            //{
            //    Processing.Update();
            //    if (Processing.ActionableByFrame)
            //    {
            //        Processing.Task();
            //    }
            //}
            //Processing.Update();


            Processing.Update();     
            IsPushable = Processing.IsPushAllowed;
            if (Processing.IsBasicCompleted)
            {
                FinishCurrentTask();
                PerformNextAction();
            }
            NextFrameInProcessing();

        }
        
    }

    private void NextFrameInProcessing()
    {
        foreach (var item in observers)
        {
            item.OnNext(Processing);
        }
    }

    //private void Process(T attack)
    //{
    //    IsPushable = false;
    //    Processing = attack;
    //    Processing.RegisterAttack += c_ReadyNextAttack;
    //    Processing.ExecuteNextAction += c_PerformNextAction;
    //    Processing.Execute();
    //}

    internal bool ForcePush(Basic action, bool priority = false, EventHandler? evt = null, EventHandler? evt2 = null)
    {
        
        if(Processing != null)
        {
            FinishCurrentTask();
            AllowPush();
            return Push(action, priority, evt, evt2);
        }
        else
        {
            return Push(action, priority, evt, evt2);
        }
    }

    //private void Process()
    //{
    //    if(Processing == null)
    //    {

            
    //        Processing = NextInQueue;
    //        processingPushConditions = queuePushConditions;
    //        processingEndConditions = queueEndConditions;
    //        //attackRegisterableTime = this.attack.Item2;
    //        //attackEndTime = this.attack.Item3;
    //        NextInQueue = null;
    //        if(Processing == null)
    //        {
    //            Debug.Log("processsing is null");
    //        }
            
    //        //Processing.RegisterAttack += c_ReadyNextAttack;
    //        //Processing.ExecuteNextAction += c_PerformNextAction;
            
    //        Processing.Execute();
    //        //Reset();
    //    }
        
    //}

    //public void Process()//will get deleted
    //{
    //    waitingQueue.StartedProcessing(waitingQueue.NextInQueue);
    //    waitingQueue.SetProcessedEvent(this);
    //    Process(waitingQueue.NextInQueue);
    //    waitingQueue = null;

    //}

    public void ProcessActionInQueue()//will get deleted
    {
        if (Processing == null && queue != null)
        {
            ProcessAction(queue.NextInQueue, queue.Observers.ToList());
            
        }

    }
    //private void ProcessActionInQueue(Basic action, List<IActionObserver> observers)
    //{
    //    Processing = action;
    //    this.observers = observers;
    //    Processing.Execute();
    //    taskAdvancer.Push(this, action.IsProcessedInFixedUpdate);
    //}



    private void Process(Basic action)//will get deleted
    {
        if(Processing == null)
        {
            Processing = action;
            Processing.Execute();
            taskAdvancer.Push(this, action.IsProcessedInFixedUpdate);
        }
    }
    public void SetProcessedEvent(EventHandler evt)//will get deleted
    {
        OnActionFinished = evt;
    }

    public void Reset()
    {
        AllowPush();
        //Processing = null;
        queue = null;
    }

    //public IDisposable Subscribe(IObserver<Basic> observer)
    //{
        
    //    return Subscribe((IAttackHolder) observer);
    //}

    //public IDisposable Subscribe(IAttackHolder observer)
    //{
    //    observers.Add(observer);
    //    return new Unsbscription<Basic>(observers, observer);
    //}

    internal void AllowPush()
    {
        IsPushable = true;
    }

    //internal void FinishCurrentTask()
    //{
    //    Debug.Log("performing next action");
    //    if(Processing!= null)
    //    {
    //        Processing.StopAction();
    //        //UnRegisterEvent();
    //        var temp = Processing;
    //        Processing = null;
    //        OnActionFinished?.Invoke(null, null);//will get deleted check first
    //        InformObservers(temp);
    //    }
        
        
        
    //    //IsPushable = false;
    //    //if (NextInQueue != null)
    //    //{
    //    //    //var temp = attack;
    //    //    //attack = null;
    //    //    //IsPushable = true;
    //    //    Debug.Log("processing next action");
    //    //    Process();
    //    //}
    //    //else
    //    //{
    //    //    Debug.Log("going idle");
    //    //    //Processing = null;
    //    //    Reset();
    //    //    GoingIdle?.Invoke(null, null);//may get deleted
    //    //}
    //}

    //private void InformObservers(Basic action)
    //{
    //    foreach (var item in observers)
    //    {
    //        item.OnCompleted(action);
    //    }
    //}

    internal void PerformNextAction()
    {
        if (queue != null)
        {
            //var temp = attack;
            //attack = null;
            //IsPushable = true;
            Debug.Log("processing next action");
            ForcePush(queue.NextInQueue, queue.Observers);
        }
        else
        {
            Debug.Log("going idle");
            //Processing = null;
            Reset();
            GoingIdle?.Invoke(null, null);//may get deleted maybe not
        }
    }

    internal bool IsQueueFull()
    {
        if(queue != null)
        {
            return true;
        }
        return false;
    }

    


    //public bool IsAttackAccessible()
    //{

    //}

}

public interface IAttackProcessor : IObservable<Basic>
{
    IDisposable Subscribe(IActionObserver observer);
    List<IActionObserver> CurrentTaskObservers();
    void FinishCurrentTask();
    public IHandler[]? ForcePush(Basic action, IActionObserver[]? observers);
}

public interface ITimeAffected
{
    public void AdvanceTime(float time);
}

public class Queue //outdated will get deleted
{
    public Basic NextInQueue { get; internal set; }
    private event EventHandler ProcessingAction;
    public event EventHandler OnActionProcessed;

    public Queue(Basic basic, EventHandler handler, EventHandler onProcessedAction)
    {
        NextInQueue = basic;
        ProcessingAction = handler;
        OnActionProcessed = onProcessedAction;
    }

    internal void StartedProcessing(Basic basic)//gets called when the processor is started processing action in nextInQueue
    {
        ProcessingAction?.Invoke(basic, null);
    }

    internal void SetProcessedEvent(AttackProcessor processor)
    {
        processor.SetProcessedEvent(OnActionProcessed);//sets the event that will be executed after the processor has done processing the current task
    }
}

public class ProcessorQueue //outdated will get deleted
{
    public Basic NextInQueue { get; internal set; }
    public IActionObserver[]? Observers { get; set; }

    public ProcessorQueue(Basic basic, IActionObserver[]? observers)
    {
        NextInQueue = basic;
        Observers = observers;
    }

}

public class Unsbscription<T> : IDisposable
{

    protected List<IActionObserver> observers1;
    protected IActionObserver observer1;

    public Unsbscription(List<IActionObserver> observers1, IActionObserver observer1)
    {
        this.observers1 = observers1;
        this.observer1 = observer1;
    }

    public void Dispose()
    {
        observers1.RemoveAt(observers1.IndexOf(observer1));
        //for (int i = 0; i < observers1.Count; i++)
        //{
        //    if (observers1[i] == observer1)
        //    {
        //        observers1.RemoveAt(i);
        //    }
        //}
    }


    
}

public class Handler<T> : Unsbscription<T>, IHandler
{
    protected IAttackProcessor AttackProcessor;
    public Handler(List<IActionObserver> observers1, IActionObserver observer1, IAttackProcessor processor) : base(observers1, observer1)
    {
        AttackProcessor = processor;
    }

    public void End()
    {
        if(observers1.IndexOf(observer1) >= 0)
        {
            if (AttackProcessor.CurrentTaskObservers().Contains(observer1))
            {
                AttackProcessor.FinishCurrentTask();
                observers1.Clear();
            }
            
        }
    }
}

public interface IHandler : IDisposable
{
    public void End();
}

public class Processor : IAttackProcessor, ITimeAffected
{
    protected TaskAdvancer taskAdvancer;
    protected List<IActionObserver> observers;
    protected int id;
    public Basic? Processing { get; set; }
    

    public Processor(TaskAdvancer advancer)
    {
        taskAdvancer = advancer;
        observers = new();
    }
    public IDisposable Subscribe(IActionObserver observer)
    {
        
        observers.Add(observer);
        return new Unsbscription<Basic>(observers, observer);
    }

    public IDisposable Subscribe(IObserver<Basic> observer)
    {
        return Subscribe((IAttackHolder)observer);
    }

    public List<IActionObserver> CurrentTaskObservers()
    {
        return observers;
    }

    public IHandler[]? ForcePush(Basic action, IActionObserver[]? observers)
    {
        if(Processing != null)
        {
            FinishCurrentTask();
        }
        ProcessAction(action, observers.ToList());
        return GetHandlers(observers);
    }
    private IHandler CreateHandler(IActionObserver observer, IActionObserver[] observers)
    {
        return new Handler<Basic>(observers.ToList(), observer, this);
    }
    protected IHandler[]? GetHandlers(IActionObserver[]? observers)
    {
        if (observers != null && observers.Length > 0)
        {
            IHandler[] handlers = new IHandler[observers.Length];
            for (int i = 0; i < observers.Length; i++)
            {
                handlers[i] = CreateHandler(observers[i], observers);
            }
            return handlers;
        }
        return null;
    }
    protected void ProcessAction(Basic action, List<IActionObserver> observers)
    {
        Processing = action;
        this.observers = observers;
        Processing.Execute();
        taskAdvancer.Push(this, action.IsProcessedInFixedUpdate);
    }
    public void FinishCurrentTask()
    {
        Debug.Log("performing next action");
        if (Processing != null)
        {
            Processing.StopAction();
            //UnRegisterEvent();
            var temp = Processing;
            Processing = null;
            //OnActionFinished?.Invoke(null, null);//will get deleted check first
            InformObservers(temp);
        }


    }

    protected virtual void OnActionFinised()
    {
        FinishCurrentTask();
    }

    private void InformObservers(Basic action)
    {
        foreach (var item in observers)
        {
            item.OnCompleted(action);
        }
    }
    public void AdvanceTime(float time)
    {
        if (!Processing!.IsBasicCompleted)
        {
            //Processing.AdvanceTime();
            Processing.Duration += time;
        }
        else
        {
            OnActionFinised();
        }
    }
}


