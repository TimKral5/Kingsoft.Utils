using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace Kingsoft.Utils.Nuget.SpectreConsole
{
    public class ExplorerSelectionPrompt : IPrompt<string>
    {
        public class ResultHandler
        {
            private ExplorerSelectionPrompt Prompt { get; set; }
            public bool Goto(string[] path) => Prompt._Goto(path);
            public void Resolve() => Prompt._Resolve();
            public void Back() => Prompt._Back();

            public ResultHandler(ExplorerSelectionPrompt prompt)
            {
                Prompt = prompt;
            }
        }

        public class Item
        {
            public Item()
            {
                Name = "";
                Action = (x, y) => { };
                Children = new List<Item>();
                Parent = this;
            }

            public string Name { get; set; }
            public Action<string, ResultHandler> Action { get; set; }
            public List<Item> Children { get; set; }
            public Item Parent { get; set; }
        }

        private SelectionPrompt<string> Prompt { get; set; }
        private ResultHandler Handler { get; set; }
        private Item Origin { get; set; }
        private Item Directory { get; set; }
        private bool IsResolved { get; set; }

        private string BackButton { get; set; }
        private string Frame { get; set; }
        private Item NewDir { get; set; }

        public ExplorerSelectionPrompt(string backBtn = null, string frame = "{0}")
        {
            Prompt = null;
            Handler = new ResultHandler(this);
            Origin = new Item();
            Directory = Origin;
            BackButton = backBtn;
            Frame = frame;
        }

        public void SetPath(string[] path, Action<string, ResultHandler> action)
        {
            Item dir = Origin;
            int i = 0;
            int len = path.Length;
            path.All(loc =>
            {
                i++;
                try
                {
                    dir = dir.Children.First(el => el.Name == loc);
                }
                catch
                {
                    Item newItem = new Item
                    {
                        Name = loc,
                        Parent = dir
                    };

                    if (i == len)
                        newItem.Action = action;

                    dir.Children.Add(newItem);
                    dir = newItem;
                }
                return true;
            });
        }

        public void SetPath(string path, Action<string, ResultHandler> action) 
            => SetPath(path.Split('/').Skip(1).ToArray(), action);

        public bool _Goto(string[] path)
        {
            Item dir = Origin;
            bool exists = path.All(loc =>
            {
                Item item = dir.Children.First(el => el.Name == loc);
                if (item != null)
                {
                    dir = item;
                    return true;
                }
                return false;
            });

            if (exists)
            {
                NewDir = dir;
                return true;
            }
            return false;
        }

        public bool _Goto(string path) => _Goto(path.Split('/').Skip(1).ToArray());
        public void _Resolve() => IsResolved = true;
        public void _Back() => NewDir = Directory.Parent;

        public string Show(IAnsiConsole console)
        {
            NewDir = null;
            IsResolved = false;
            string result = "";

            while(!IsResolved)
            {
                Dictionary<string, Item> choices = new Dictionary<string, Item>();
                Directory.Children.ForEach(item => choices.Add(string.Format(Frame, item.Name), item));

                Prompt = new SelectionPrompt<string>();
                if (BackButton != null)
                    Prompt.AddChoice(BackButton);
                Prompt.AddChoices(choices.Keys);
                result = Prompt.Show(console);

                if (BackButton != null && result == BackButton)
                    Directory = Directory.Parent;
                else
                {
                    Item _result = Directory.Children.First(loc => string.Format(Frame, loc.Name) == result);
                    Directory = _result;
                    _result.Action.Invoke(result, Handler);
                }
            }

            string[] frameParts = Frame.Split(new string[] { "{0}" }, StringSplitOptions.None);
            return result.Substring(frameParts[0].Length, 
                result.Length - frameParts[0].Length - frameParts[1].Length);
        }

        public Task<string> ShowAsync(IAnsiConsole console, CancellationToken cancellationToken)
            => Prompt.ShowAsync(console, cancellationToken);
    }
}
