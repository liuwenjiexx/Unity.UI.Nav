using System;
using UnityEngine;
namespace Unity.UI.Navs
{
    public class Dialog : Navigable
    {
        public new DialogModel Model
        {
            get => base.Model as DialogModel;
        }

        public override void OnLoad()
        {
            base.OnLoad();
            Model.OnClosing += (button)=>
            {
                Close(button);
            };
        }

        public override void OnNavigationFrom(NavContext from)
        {
            Model.ResetDiry();
            base.OnNavigationFrom(from);
        }

        protected override void Update()
        {
            base.Update();

            if (Model == null)
                return;

            if (Model.IsDiried)
            {
                Model.ResetDiry();
                Refresh();
            } 
        }

        protected virtual void Click(int button)
        {
            OnClick(button);
            if (Model.OnClick != null)
            {
                Model.OnClick(button);
            }
            else
            {
                Close(button);
            }
        }

        protected virtual void OnClick(int button)
        {

        }
        protected virtual void OnClose()
        {
        }


        public virtual void Close(int button)
        {

            Nav.BackTo(ViewId);
            Nav.Remove(ViewId);
            OnClose();
            Model.Close(button);
            //if (lastSelected && lastSelected.activeInHierarchy)
            //{
            //    EventSystem.current.SetSelectedGameObject(lastSelected);
            //}

        }

        protected class DialogScope : IDisposable
        {
            private bool disposed;
            public DialogModel model;

            public DialogScope(DialogModel model)
            {
                this.model = model;
            }
            public void Dispose()
            {
                if (!disposed)
                {
                    disposed = true;
                    model?.Close(0);
                }
            }

        }
    }
}