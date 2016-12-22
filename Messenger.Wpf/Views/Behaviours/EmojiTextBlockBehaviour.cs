using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Media.Imaging;
using VkData.Helpers;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Messenger.Wpf.Views.Behaviours
{
    public class EmojiTextBehavior : Behavior<TextBlock>
    {
        private static Random _random = new Random();
        private static readonly Dictionary<string, string> _emojiPathEncoded =
           new Dictionary<string, string>();
        static EmojiTextBehavior()
        {
            var path = Environment.CurrentDirectory.JoinPath("Vk-emoji");

            var info = Directory.GetFiles(path);
            foreach (var t in info)
            {
                var fileName = Path.GetFileName(t);
                if (fileName == null) return;
                var v = fileName.Replace(".png", string.Empty);
                _emojiPathEncoded.Add(HttpUtility.HtmlDecode(v), t);
            }
        }
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        /// <summary>
        /// DependencyProperty to allow binding of our DataObject
        /// </summary>
        public static readonly DependencyProperty DataObjectProperty = DependencyProperty.Register(
            "DataObject", typeof(string), typeof(EmojiTextBehavior), null);

        public string DataObject
        {
            get { return (string)GetValue(DataObjectProperty); }
            set { SetValue(DataObjectProperty, value); }
        }

        public static readonly DependencyProperty ConditionProperty = DependencyProperty.Register(
         "Condition", typeof(bool), typeof(EmojiTextBehavior), null);

        private readonly Regex _regexEmoji = new Regex(
                @"\uD83C[\uDF00-\uDFFF]|\uD83D[\uDC00-\uDEFF]|[\u2600-\u26FF]", RegexOptions.Compiled);

        public bool Condition
        {
            get { return (bool)GetValue(ConditionProperty); }
            set { SetValue(ConditionProperty, value); }
        }

        void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Condition)
            {
                ((TextBlock) sender).Text = DataObject;
                return;
            }
           // Put the formatting logic here
            var block = (TextBlock)sender;
            var target = DataObject;
            var matchCollection = _regexEmoji.Matches(target);
            var matchDictionary = new Dictionary<int, Match>();
            for (var i = 0; i < matchCollection.Count; i++)
            {
                matchDictionary.Add(matchCollection[i].Index, matchCollection[i]);
            }

            int step;
            for (var i = 0; i < target.Length; i+=step)
            {
                if (!matchDictionary.ContainsKey(i))
                {
                    step = 1;
                    block.Inlines.Add(target[i].ToString());
                    continue;
                }

                var match = matchDictionary[i];
                var ba = Encoding.BigEndianUnicode.GetBytes(match.Value);
                var hexString = BitConverter.ToString(ba);
                hexString = hexString.Replace("-", "");

                step = match.Length;
                if (!_emojiPathEncoded.ContainsKey(hexString))
                    continue;
                var path =_emojiPathEncoded[hexString];
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(path);
                bi.EndInit();

                var img = new Image
                {
                    Source = bi,
                    Height = 16,
                    Width = 16
                };
                var c = new InlineUIContainer(img);
                block.Inlines.Add(c);
            }
        }
    }

}