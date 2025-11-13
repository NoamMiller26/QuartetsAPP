using Quartets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartets.ModelLogic
{
    public class MainPageML : MainPageModel
    {


        public override void ShowInstructionsPrompt(object obj)
        {
            Application.Current!.MainPage!.DisplayAlert(Strings.Instructions, Strings.InsructionsTxt, Strings.Ok);
        }
    }
}