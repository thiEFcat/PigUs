﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Ingame
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        public DialogueDisplayer Displayer;
        public DialogueClickReceiver Receiver;
        public DialogueInteractor Interactor;
        public Sprite DefaultSprite;

        private void Awake()
        {
            Instance = this;
        }

        public void ShowImage(string imageKey)
        {
            Displayer.CaseImage.sprite = Resources.Load<Sprite>($"Images/{imageKey}");
            Displayer.CaseImage.type = UnityEngine.UI.Image.Type.Simple;
            Displayer.CaseImage.preserveAspect = true;
        }

        public async Task ShowDialogue(string talker, string context, bool unskippable)
        {
            Receiver.AllowedToReceive = !unskippable;
            Displayer.UnskipIndicator.SetActive(unskippable);
            await Displayer.DisplayContext(talker, context);
            Receiver.AllowedToReceive = false;
        }

        public async Task<int> ShowInteraction(string[] sels)
        {
            int mynum = IngameManager.MasterGameNum;

            Receiver.AllowedToReceive = false;
            Interactor.gameObject.SetActive(true);
            Interactor.Show(sels);
            await new WaitUntil(() => (Interactor.HasResult || mynum != IngameManager.MasterGameNum));
            if (mynum != IngameManager.MasterGameNum)
                return 0;
            Interactor.gameObject.SetActive(false);
            return Interactor.Result;
        }

        public void CleanDialogue()
        {
            Displayer.Talker.text = "";
            Displayer.Context.text = "";
            Displayer.CaseImage.sprite = DefaultSprite;
        }
    }
}