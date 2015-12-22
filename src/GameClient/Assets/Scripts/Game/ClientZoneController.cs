using System.Linq;
using Domain;
using TrackableData;
using UnityEngine;
using UnityEngine.UI;

public class ClientZoneController : ZoneControllerClientBase, IZoneControllerClientHandler
{
    public override void OnTrackableDataChanged(int index, ITracker tracker)
    {
        if (index == 0)
            OnDataChanged((TrackablePocoTracker<IZoneControllerData>)tracker);
    }

    private void OnDataChanged(TrackablePocoTracker<IZoneControllerData> tracker)
    {
        foreach (var i in tracker.ChangeMap)
        {
            if (i.Key == TrackableZoneControllerData.PropertyTable.State)
                OnStateChanged();
        }
    }

    private void OnStateChanged()
    {
        Debug.Log("ClientZoneController State: " + Data.State);

        var stateText = GetStateText();
        if (stateText != null)
            stateText.text = Data.State.ToString();

        switch (Data.State)
        {
            case ZoneState.Ready:
                OnGameReady();
                break;
        }
    }

    private void OnGameReady()
    {
        var snakes = Zone.GetEntities<ClientSnake>().ToList();

        var controlPad = GetSnakeControlPad();
        if (controlPad != null)
            controlPad.Snake = snakes.FirstOrDefault(s => s.IsControllable);

        if (snakes.Count > 0)
            snakes[0].ScoreText = GetPlayerScoreText(1);
        if (snakes.Count > 1)
            snakes[1].ScoreText = GetPlayerScoreText(2);
    }

    // UI Accessor

    private static Text GetStateText()
    {
        return GameObject.Find("StateText").GetComponent<Text>();
    }

    private static SnakeControlPad GetSnakeControlPad()
    {
        return GameObject.Find("ControlPad").GetComponent<SnakeControlPad>();
    }

    private static Text GetPlayerScoreText(int playerId)
    {
        var parent = GameObject.Find("Player" + playerId);
        return parent.transform.Find("Score").GetComponent<Text>();
    }
}
