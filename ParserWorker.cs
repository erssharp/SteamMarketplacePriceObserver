using AngleSharp.Parser.Html;
using SteamMarketplacePriceObserver.Core.Steam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace SteamMarketplacePriceObserver.Core
{
    class ParserWorker<T> where T : class
    {
        bool isActive;
        string id = null;

        IParser<T> parser;
        IParserSettings settings;

        HtmlLoader loader;

        public event Action<object, T> OnNewData;
        public event Action<object> OnCompleted;

        public IParser<T> Parser { get { return parser; } set { parser = value; } }
        public IParserSettings Settings { get { return settings; } set { settings = value; loader = new HtmlLoader(value); } }
        public string ID { get { return id; } set { id = value; } }


        public bool IsActive { get { return isActive; } }

        public ParserWorker(IParser<T> parser, string id)
        {
            this.id = id;
            this.parser = parser;
        }

        public ParserWorker(IParser<T> parser, IParserSettings settings, string id) : this(parser, id)
        {
            this.settings = settings;
        }

        public void Start()
        {
            isActive = true;
            Worker();
        }

        public void Abort()
        {
            isActive = false;
        }

        private async void Worker()
        {
            while (isActive)
            {
                var source = await loader.GetSourceByPageId(id); // скачиваем html код страницы 
                                                                 // в виде строки
                var domParser = new HtmlParser();

                var result = source as T;                        // приводим строку к T 
                                                                 // (в данном случае это необязательно)

                OnNewData?.Invoke(this, result);                 // вызываем событие OnNewData

                for(int i = 0; i < 60; i++)                      // задержка
                {
                    if (!isActive)
                        break;
                    await Task.Delay(500);
                }
            }
            OnCompleted?.Invoke(this);                           // вызываем событие OnCompleted
        }
    }
}
