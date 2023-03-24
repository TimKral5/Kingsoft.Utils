using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingsoft.Utils.TypeExtensions.DictionaryExt;
using Microsoft.SqlServer.Server;

namespace Kingsoft.Utils.Text
{
    public class WordProcessor
    {
        public struct WordProcessorData<T>
        {
            public Dictionary<string, object> States { get; set; }
            public string[] ProcessedWords { get; set; }
            public string ProcessedText { get; set; }
            public T CurrentElement { get; set; }
            public string RemainingText { get; set; }

        }

        private Dictionary<string, object> States { get; set; }
        private Dictionary<string, Action<WordProcessorData<string>>[]> WordListeners { get; set; }
        private Dictionary<char, Action<WordProcessorData<char>>[]> CharListeners { get; set; }

        public object this[string index] 
        {
            get => States.Get(index);
            set => States.Set(index, value);
        }

        public WordProcessor(char[] wordSeparators = null)
        {
            States = new Dictionary<string, object>();
            WordListeners = new Dictionary<string, Action<WordProcessorData<string>>[]>();
            CharListeners = new Dictionary<char, Action<WordProcessorData<char>>[]>();

            if (wordSeparators == null)
                wordSeparators = new char[] { ' ', '\t', '\r', '\n' };
            States.Set("WordSeparators", wordSeparators);
        }

        public void Decode(string text)
        {
            string processedStr = "";
            List<string> processedWords = new List<string>();

            string word = "";

            void invoke<T>(Action<WordProcessorData<T>> func, T current)
            {
                func(new WordProcessorData<T>
                {
                    ProcessedText = processedStr,
                    ProcessedWords = processedWords.ToArray(),
                    CurrentElement = current,
                    RemainingText = text,
                    States = States
                });
            }

            while (text.Length > 0)
            {
                char ch = text[0];
                text = text.Substring(1);
                processedStr += ch;

                CharListeners.All(listener =>
                {
                    if (listener.Key == ch)
                        listener.Value.All(func => { invoke(func, ch); return true; });
                    return true;
                });

                if (!((char[])States["WordSeparators"]).Any(_ch => _ch == ch) && text.Length > 0)
                    word += ch;
                else
                {
                    processedWords.Add(word);
                    WordListeners.All(listener =>
                    {
                        Console.WriteLine("key: {0}", listener.Key);
                        if (listener.Key == word)
                            listener.Value.All(func => { invoke(func, word); return true; });
                        return true;
                    });
                    word = "";
                }

                Console.WriteLine(word);
                
            }
        }

        public void AddWordListener(string str, Action<WordProcessorData<string>> cb) => WordListeners.Set(str, cb);
        public void RemoveWordListener(string str, Action<WordProcessorData<string>> cb) => WordListeners.Remove(str);

        public void AddCharListener(char ch, Action<WordProcessorData<char>> cb) => CharListeners.Set(ch, cb);
        public void RemoveCharListener(char ch, Action<WordProcessorData<char>> cb) => CharListeners.Remove(ch);
    }
}
