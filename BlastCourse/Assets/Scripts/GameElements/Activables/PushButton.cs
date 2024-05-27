using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushButton : ActivableButton, IInteractable
{
    [Space(5), Header("Push Button Properties"), Space(3)]
    public bool LockAfterFirstPress;
    public float Cooldown;

    private PlayerInteract _interactor;

    public virtual void SetInteraction(bool set, PlayerInteract interactor)
    {
        if (set)
        {
            _interactor = interactor;
            Press(true);

            if (!ResetOnUnpress)
            {
                TryCancelInteraction();
                Locked = true;

                if (!LockAfterFirstPress)
                {
                    this.Invoke(() => Press(false), Cooldown * 0.5f);
                    this.Invoke(() => Locked = false, Cooldown);
                }
            }
        }
        else
        {
            TryCancelInteraction();
            _interactor = null;
            if(ResetOnUnpress && !LockAfterFirstPress) Press(false);
        }
    }

    public void OnInteractButtonUp()
    {
        if (ResetOnUnpress)
        {
            TryCancelInteraction();
            if (!LockAfterFirstPress) Press(false);
        }
    }

    public void TryCancelInteraction()
    {
        if (_interactor != null) _interactor.CancelCurrentInteraction();
    }

    protected void ResetButton()
    {
        Locked = false;
        Press(false);
    }

    public override void Press(bool press)
    {
        base.Press(press);

        if (LockAfterFirstPress)
        {
            Locked = true;
            return;
        }

        if (!press)
        {
            Locked = true;
            this.Invoke(() => Locked = false, Cooldown);
        }
    }
}
