using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CastleClub.FrontEnd.Models
{
    public class BaseVM
    {
        public bool HasErrors { get; set; }

        public SelectListItem CreateListItem(object value)
        {
            return CreateListItem(value, value.ToString());
        }

        public SelectListItem CreateListItem(object value, string text)
        {
            SelectListItem item = new SelectListItem();
            item.Text = text;
            item.Value = value.ToString();

            return item;
        }

        public SelectListItem GetSelectListItem(string value, string text, bool selected)
        {
            SelectListItem item = new SelectListItem();
            item.Value = value;
            item.Text = text;
            if (selected)
            {
                item.Selected = selected;
            }
            return item;
        }

        private SelectList GetSelectList(List<SelectListItem> items)
        {
            return GetSelectList(items, null);
        }

        private SelectList GetSelectList(List<SelectListItem> items, string selectedValue)
        {
            return new SelectList(items, "Value", "Text", selectedValue);
        }

        public SelectList GetSelectList(IEnumerable items, string value, string text, string emptyText)
        {
            return GetSelectList(items, value, text, emptyText, null);
        }

        public SelectList GetSelectList(IEnumerable items, string value, string text, string emptyText, string selectedValue)
        {
            List<SelectListItem> finalItems = new List<SelectListItem>();
            if (emptyText != null)
            {
                finalItems.Add(GetSelectListItem("", emptyText, false));
            }
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item is string || item is int)
                    {
                        string thisValue = item.ToString();
                        finalItems.Add(GetSelectListItem(thisValue, thisValue, thisValue == selectedValue));
                    }
                    else if (item is bool)
                    {
                        string thisValue = (bool)item ? "Yes" : "No";
                        finalItems.Add(GetSelectListItem(item.ToString(), thisValue, thisValue == selectedValue));
                    }
                    else
                    {
                        string thisValue = item.GetType().GetProperty(value).GetValue(item, null).ToString();
                        finalItems.Add(GetSelectListItem(thisValue, item.GetType().GetProperty(text).GetValue(item, null).ToString(), thisValue == selectedValue));
                    }
                }
            }
            return GetSelectList(finalItems, selectedValue);
        }

        public SelectList GetSelectList<T>()
        {
            return GetSelectList<T>(null);
        }

        public SelectList GetSelectList<T>(string emptyText)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            List<SelectListItem> finalItems = new List<SelectListItem>();
            if (emptyText != null)
            {
                finalItems.Add(GetSelectListItem("", emptyText, false));
            }

            string prefix = typeof(T).ToString();
            Type t = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(x => x.FullName == prefix + "Helper").First();
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                string text = (string)t.GetMethod("GetString").Invoke(null, new object[] { item });
                finalItems.Add(GetSelectListItem(item.ToString(), text, false));
            }
            return GetSelectList(finalItems);
        }

        public SelectList GetSelectList<T>(string emptyText, string selectedvalue)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            List<SelectListItem> finalItems = new List<SelectListItem>();
            if (emptyText != null)
            {
                finalItems.Add(GetSelectListItem("", emptyText, false));
            }

            string prefix = typeof(T).ToString();
            Type t = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(x => x.FullName == prefix + "Helper").First();
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                string text = (string)t.GetMethod("GetString").Invoke(null, new object[] { item });
                finalItems.Add(GetSelectListItem(item.ToString(), text, item.ToString().ToUpper()==selectedvalue.ToUpper()));
            }
            return new SelectList(finalItems, "Value", "Text", "MASTERCARD");
        }

    }
}