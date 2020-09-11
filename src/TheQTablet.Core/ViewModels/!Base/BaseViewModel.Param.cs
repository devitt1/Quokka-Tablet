using System;
using System.Collections.Generic;
using System.Text;
using MvvmCross.ViewModels;

namespace TheQTablet.Core.ViewModels
{
    public abstract class BaseViewModel<TParameter> : BaseViewModel, IMvxViewModel<TParameter>
    {
        public abstract void Prepare(TParameter parameter);
    }
}
