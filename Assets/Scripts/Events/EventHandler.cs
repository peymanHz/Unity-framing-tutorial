using System;
using System.Collections.Generic;
using UnityEngine.UI;

public delegate void MovmentDelegate(float inputX, float inputY, 
    bool isWalking, bool isRunning, bool isIdle, bool isCarrying,
    ToolEffect toolEffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleRight, bool idleLeft, bool idleUp, bool idleDown);

public static class EventHandler
{
    //drop selected item event
    public static event Action DropSelectedItemEvent;
    
    public static void CallDropSelectedItemEvent()
    {
        if (DropSelectedItemEvent != null)
        {
            DropSelectedItemEvent();
        }
    }

    //Inventory Event
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdateEvent;

    public static void CallInventoryUpdateEvent(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (InventoryUpdateEvent != null)
        {
            InventoryUpdateEvent(inventoryLocation, inventoryList);
        }
    }

    // Movemnt Event

    public static event MovmentDelegate MovmentEvent;

    // movment Event Call For Publishers

    public static void CallMovemntEvent(float inputX, float inputY,
    bool isWalking, bool isRunning, bool isIdle, bool isCarrying,
    ToolEffect toolEffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleRight, bool idleLeft, bool idleUp, bool idleDown)
    {
        if (MovmentEvent != null)
            MovmentEvent(inputX,inputY,
                isWalking,isRunning,isIdle,isCarrying,
                toolEffect,
                isUsingToolRight,isUsingToolLeft,isUsingToolUp,isUsingToolDown,
                isLiftingToolRight,isLiftingToolLeft,isLiftingToolUp,isLiftingToolDown,
                isPickingRight,isPickingLeft,isPickingUp,isPickingDown,
                isSwingingToolRight,isSwingingToolLeft,isSwingingToolUp,isSwingingToolDown,
                idleRight,idleLeft,idleUp,idleDown);
        
    }


    //time event

    //advance game minute
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent;

    public static void CallAdvanceGameMinuteEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfweek, int gameHour, int gameMinute, int gameSecod)
    {
        if (AdvanceGameMinuteEvent != null)
        {
            AdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfweek, gameHour, gameMinute, gameSecod);
        }
    }

    //advance game Hour
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;

    public static void CallAdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfweek, int gameHour, int gameMinute, int gameSecod)
    {
        if (AdvanceGameHourEvent != null)
        {
            AdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfweek, gameHour, gameMinute, gameSecod);
        }
    }

    //advance game Day
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;

    public static void CallAdvanceGameDayEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfweek, int gameHour, int gameMinute, int gameSecod)
    {
        if (AdvanceGameDayEvent != null)
        {
            AdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfweek, gameHour, gameMinute, gameSecod);
        }
    }

    //advance game Season
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;

    public static void CallAdvanceGameSeasonEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfweek, int gameHour, int gameMinute, int gameSecod)
    {
        if (AdvanceGameSeasonEvent != null)
        {
            AdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfweek, gameHour, gameMinute, gameSecod);
        }
    }

    //advance game Year
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;

    public static void CallAdvanceGameYearEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfweek, int gameHour, int gameMinute, int gameSecod)
    {
        if (AdvanceGameYearEvent != null)
        {
            AdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfweek, gameHour, gameMinute, gameSecod);
        }
    }

    // scene load Events - in the order they happen

    //before scene unload fade out event
    public static event Action BeforeSceneUnloadFadeOut;

    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if (BeforeSceneUnloadFadeOut != null)
        {
            BeforeSceneUnloadFadeOut();
        }
    }

    //before scene unload event
    public static event Action BeforeSceneUnloadEvent;

    public static void CallBeforeSceneUnloadEvent()
    {
        if (BeforeSceneUnloadEvent != null)
        {
            BeforeSceneUnloadEvent();
        }
    }

    //after scene loaded event
    public static event Action AfterSceneLoadEvent;

    public static void CallAfterSceneLoadEvent()
    {
        if (AfterSceneLoadEvent != null)
        {
            AfterSceneLoadEvent();
        }
    }

    //after scene load fade in event
    public static event Action AfterSceneLoadFadeInEvent;

    public static void CallAfterSceneLoadFadeInEvent()
    {
        if (AfterSceneLoadFadeInEvent != null)
        {
            AfterSceneLoadFadeInEvent();
        }
    }


}