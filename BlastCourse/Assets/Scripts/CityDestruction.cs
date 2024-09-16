using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMethods;
using UnityEngine.Events;
using UnityEngine.Playables;

public class CityDestruction : ActivableBase
{
    private int _deliveries;
    public PlayableDirector _Director;
    public float _SecondDelDelay;
    public UnityEvent _OnSecondDelivery;
    public UnityEvent _OnThirdDelivery;
    private bool _entered;

    [ActivableAction]
    public void Deliver(bool set)
    {
        if (set) _deliveries++;
        if (_deliveries == 2) this.Invoke(() => _OnSecondDelivery.Invoke(), _SecondDelDelay);
        else if (_deliveries >= 3) _OnThirdDelivery.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_entered) return;
        _Director.Play();
        _entered = true;
    }
}


