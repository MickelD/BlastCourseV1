using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LandParticles : MonoBehaviour
{
    [SerializeField] GameObject _particlePrefab;
    [SerializeField] int _startingInactiveParticles;
    List<GameObject> _inactiveParticles;
    ParticleSystem.MainModule _main;
    ParticleSystem.Burst _burst;
    WaitForSeconds _active;
    [Space(10)]
    [SerializeField] float _minSpeed;
    [SerializeField] float _maxSpeed;
    [SerializeField] int _minParticleCount;
    [SerializeField] int _maxParticleCount;
    [SerializeField] float _minFallSpeedThreshold;
    [SerializeField] float _maxFallSpeedThreshold;

    private void Start()
    {
        _inactiveParticles = new List<GameObject>();
        if(_particlePrefab != null) 
            for(int i = 0; i < _startingInactiveParticles; i++)
            {
                GameObject GO = Instantiate(_particlePrefab, transform.position, Quaternion.Euler(Vector3.right * 90), transform);
                GO.SetActive(false);
                _inactiveParticles.Add(GO);
            }
        
    }

    private void OnEnable()
    {
        EventManager.OnPlayerLanded += FallParticle;
    }

    private void OnDisable()
    {
        EventManager.OnPlayerLanded -= FallParticle;
    }

    public void FallParticle(float fallSpeed)
    {

        if (-fallSpeed >= _minFallSpeedThreshold)
        {
            //Prepare Particle
            int index = 0;
            bool needMore = true;
            for (int i = 0; i < _inactiveParticles.Count; i++)
            {
                GameObject go = _inactiveParticles[i];
                if (!go.activeSelf)
                {
                    index = i;
                    needMore = false;
                    break;
                }
            }
            if (needMore)
            {
                GameObject GO = Instantiate(_particlePrefab, transform.position, Quaternion.Euler(Vector3.right*90), transform);
                GO.SetActive(false);
                _inactiveParticles.Add(GO);
            }
            ParticleSystem _activeParticle = _inactiveParticles[index].GetComponent<ParticleSystem>();

            //Set Particle
            if (_activeParticle != null)
            {
                float percent = Mathf.Clamp01((fallSpeed - _minFallSpeedThreshold) / (_maxFallSpeedThreshold - _minFallSpeedThreshold));

                StartCoroutine(SetActive(_activeParticle, percent));
            }
        }
        
    }

    private IEnumerator SetActive(ParticleSystem particle, float percent)
    {
        particle.gameObject.SetActive(true);
        int particleCount = (int)Mathf.Lerp(_minParticleCount, _maxParticleCount, percent);
        float particleSpeed = Mathf.Lerp(_minSpeed, _maxSpeed, percent);

        _main = particle.main;
        _main.startSpeed = particleSpeed;

        _burst = particle.emission.GetBurst(0);
        particle.emission.SetBurst(0, _burst);
        _burst.count = particleCount;
        
        particle.gameObject.transform.parent = null;
        
        particle.Play();

        yield return new WaitForSeconds(particle.main.duration);

        particle.Stop();
        particle.gameObject.transform.parent = transform;
        particle.gameObject.transform.localPosition = Vector3.zero;
        particle.gameObject.SetActive(false);
    }
}


