using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace NKKD.EDIT
{
    public class TackPointModify
    {
        public TackPointModel model;
        public TackPointViewModel viewModel;

        public void SetModels(TackPointModel model, TackPointViewModel viewModel)
        {
            this.model = model;
            this.viewModel = viewModel;
        }

    }
}