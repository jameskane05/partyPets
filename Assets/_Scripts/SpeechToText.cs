using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System;

namespace SpeechRecognition
{
    public class SpeechToText : MonoBehaviour
    {
        static SpeechToText _instance;
        public static SpeechToText instance
        {
            get
            {
                if (_instance == null)
                {
                    Init();
                }
                return _instance;
            }
        }

        public static void Init()
        {
            if (instance != null) return;
            GameObject obj = new GameObject();
            obj.name = "SpeechToText";
            _instance = obj.AddComponent<SpeechToText>();
        }

        void Awake()
        {
            _instance = this;
        }

        public Action<string> onResultCallback;

        public void Setting(string _language)
        {
            #if UNITY_IPHONE
                _TAG_SettingSpeech(_language);
            #endif
        }

        public void StartRecording()
        {
            #if UNITY_IPHONE
                _TAG_startRecording();
            #endif
        }

        public void StopRecording()
        {
            #if UNITY_IPHONE
                _TAG_stopRecording();
            #endif
        }

        #if UNITY_IPHONE
            [DllImport("__Internal")]
            private static extern void _TAG_startRecording();

            [DllImport("__Internal")]
            private static extern void _TAG_stopRecording();

            [DllImport("__Internal")]
            private static extern void _TAG_SettingSpeech(string _language);
        #endif

        public void onMessage(string _message)
        {
            Debug.Log(_message);
        }

        public void onErrorMessage(string _message)
        {
            Debug.Log(_message);
        }

        /** Called when recognition results are ready. */
        public void onResults(string _results)
        {
            if (onResultCallback != null)
                onResultCallback(_results);
        }
    }
}