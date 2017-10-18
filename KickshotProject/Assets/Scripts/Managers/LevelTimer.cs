using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class LevelTimer : MonoBehaviour
    {
        public float InitialTime = 60;


        float _time_left;           // Time left on clock in seconds
        bool _running;              // True when the Timer is actually counting down
        Text _timer_text;


        // Use this for initialization
        void Awake()
        {
            _timer_text = GameObject.Find("TimerText").GetComponent<Text>();
            Reset();
        }

        void Update()
        {
            if (_running)
            {
                _time_left -= Time.deltaTime;
                UpdateUIText();

                if (_time_left <= 0)
                {
                    GetComponent<PlayerManager>().Died();
                }
            }
        }
    
        void UpdateUIText()
        {
            _timer_text.text = _time_left.ToString("N2");
        }

        public void StartCountdown()
        {
            _running = true;
        }

        public void StopCountdown()
        {
            _running = false;
        }

        public void Reset()
        {
            _time_left = InitialTime;
            _running = false;

            UpdateUIText();
        }
    }
