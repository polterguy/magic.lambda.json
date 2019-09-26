﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System.Linq;
using Newtonsoft.Json.Linq;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.json
{
    // TODO: Sanity check. Not entirely sure it actually works for all possible permutations.
    [Slot(Name = "to-json")]
    public class ToJson : ISlot
    {
        public void Signal(ISignaler signaler, Node input)
        {
            if (input.Value != null)
                input.AddRange(input.Evaluate());

            var token = Handle(input);
            input.Clear();
            input.Value = token.ToString(Newtonsoft.Json.Formatting.None);
        }

        #region [ -- Private helper methods -- ]

        private JToken Handle(Node root)
        {
            JToken result = null;
            if (root.Children.Any(x => x.Name != "" && x.Name != "."))
            {
                // Complex object.
                var resObj = new JObject();
                foreach (var idx in root.Children)
                {
                    resObj.Add(HandleProperty(idx));
                }
                result = resObj;
            }
            else if (root.Children.Any())
            {
                // Array.
                var resArr = new JArray();
                foreach (var idx in root.Children)
                {
                    resArr.Add(HandleArray(idx));
                }
                result = resArr;
            }
            else
            {
                result = new JObject();
            }

            return result;
        }

        private JToken HandleArray(Node idx)
        {
            if (idx.Children.Any())
                return Handle(idx);
            else
                return new JValue(idx.Value);
        }

        private JProperty HandleProperty(Node idx)
        {
            if (idx.Children.Any())
                return new JProperty(idx.Name, Handle(idx));

            return new JProperty(idx.Name, idx.Value);
        }

        #endregion
    }
}
