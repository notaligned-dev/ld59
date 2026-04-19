using System;
using System.Collections.Generic;
using UnityEngine;

public class StoryObjectsProvider : MonoBehaviour
{
    [SerializeField] private StoryObjectsFromPhase[] _storyGroups;

    private byte _currentGroupIndex;

    public event Action<StoryPhase> GroupFinished;

    private void Awake()
    {
        _currentGroupIndex = 0;

        for (int i = 0; i < _storyGroups.Length; i++)
        {
            _storyGroups[i].Initalize();

            if (i == _currentGroupIndex)
            {
                _storyGroups[i].GroupDone += HandleGroupDone;
                _storyGroups[i].IsLocked = false;
            }
        }
    }

    private void OnValidate()
    {
        bool isValid = true;

        foreach (var group in _storyGroups)
            isValid = isValid && group.Validate();

        if (isValid == false)
            throw new ArgumentOutOfRangeException(nameof(_storyGroups));
    }

    private void OnDestroy()
    {
        foreach (var group in _storyGroups)
            group.Dispose();
    }

    private void HandleGroupDone(StoryPhase phaseToProgress)
    {
        _storyGroups[_currentGroupIndex].GroupDone -= HandleGroupDone;
        _storyGroups[_currentGroupIndex].IsLocked = true;
        _currentGroupIndex++;

        GroupFinished?.Invoke(phaseToProgress);

        if (_currentGroupIndex < _storyGroups.Length)
        {
            _storyGroups[_currentGroupIndex].GroupDone += HandleGroupDone;
            _storyGroups[_currentGroupIndex].IsLocked = false;
        }
    }

    [System.Serializable]
    internal class StoryObjectsFromPhase : IDisposable
    {
        [SerializeField] private List<StoryProgressionObject> _objectsToLook;
        [SerializeField] private List<StoryProgressionObject> _objectsToInteract;
        [SerializeField] private StoryPhase PhaseToProgress;

        [HideInInspector]
        public bool IsLocked;
        private Dictionary<IStoryInteractable, bool> _lookedStatus;
        private Dictionary<IStoryInteractable, bool> _interactedStatus;

        public event Action<StoryPhase> GroupDone;

        public bool Validate()
        {
            if (_objectsToLook == null || _objectsToInteract == null)
                return false;

            if (PhaseToProgress == StoryPhase.Beginning)
                return false;

            return true;
        }

        public void Initalize()
        {
            IsLocked = true;
            _lookedStatus = new Dictionary<IStoryInteractable, bool>();
            _interactedStatus = new Dictionary<IStoryInteractable, bool>();

            foreach (var obj in _objectsToInteract)
            {
                if (_interactedStatus.ContainsKey(obj) == false)
                    _interactedStatus.Add(obj, false);

                obj.Interacted += HandleInteraction;
            }

            foreach (var obj in _objectsToLook)
            {
                if (_lookedStatus.ContainsKey(obj) == false)
                    _lookedStatus.Add(obj, false);

                obj.Looked += HandleLooked;
            }
        }

        public void Dispose()
        {
            foreach (var obj in _objectsToInteract)
                obj.Interacted -= HandleInteraction;

            foreach (var obj in _objectsToLook)
                obj.Looked -= HandleLooked;

            _lookedStatus.Clear();
            _interactedStatus.Clear();
        }

        private void HandleLooked(IInteractable interactable)
        {
            if (IsLocked == false && interactable is IStoryInteractable storyObject)
            {
                if (_lookedStatus.ContainsKey(storyObject))
                {
                    storyObject.TriggerStoryLookAction();
                    _lookedStatus[storyObject] = true;
                }
            }

            IsEverythingDone();
        }

        private void HandleInteraction(IInteractable interactable)
        {
            if (IsLocked == false && interactable is IStoryInteractable storyObject)
            {
                if (_interactedStatus.ContainsKey(storyObject))
                {
                    storyObject.TriggerStoryInteraction();
                    _interactedStatus[storyObject] = true;
                }
            }

            IsEverythingDone();
        }

        private void IsEverythingDone()
        {
            if (IsLocked)
                return;

            bool isDone = true;

            foreach (var (obj, status) in _lookedStatus)
            {
                if (status == false)
                {
                    isDone = false;
                    break;
                }
            }

            foreach (var (obj, status) in _interactedStatus)
            {
                if (status == false)
                {
                    isDone = false;
                    break;
                }
            }

            if (isDone)
                GroupDone?.Invoke(PhaseToProgress);
        }
    }
}
