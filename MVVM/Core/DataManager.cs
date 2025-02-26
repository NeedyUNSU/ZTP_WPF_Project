﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ZTP_WPF_Project.MVVM.Model;

namespace ZTP_WPF_Project.MVVM.Core
{
    public static class DataManager
    {

        private static readonly string TransactionsDb = "_TransactionDB.xml";
        private static readonly string TransactionsCategoryDb = "_TransactionCategoryDB.xml";

        #region TransactionsDb
        public static List<TransactionModel>? LoadTransactions()
        {
            return LoadFromXmlFile<TransactionModel>(TransactionsDb);
        }

        public static void SaveTransactions(List<TransactionModel> transactions)
        {
            SaveToXMLFile<TransactionModel>(transactions, TransactionsDb);
        }
        #endregion

        #region TransactionsDb
        public static List<TransactionCategoryModel>? LoadTransactionCategories()
        {
            return LoadFromXmlFile<TransactionCategoryModel>(TransactionsCategoryDb);
        }

        public static void SaveTransactionCategories(List<TransactionCategoryModel> transactionsCategories)
        {
            SaveToXMLFile<TransactionCategoryModel>(transactionsCategories, TransactionsCategoryDb);
        }
        #endregion

        #region Private Functions XML Handlers

        private static List<T>? LoadFromXmlFile<T>(string settingsPath)
        {
            try
            {
                if (!File.Exists(settingsPath) || new FileInfo(settingsPath).Length == 0)
                {
                    Console.WriteLine($"{typeof(T).ToString().Substring(typeof(T).ToString().LastIndexOf('.') + 1)} Err: File has not exist!");
                    return new List<T>();
                }

                var serializer = new XmlSerializer(typeof(List<T>));
                using (var reader = new StreamReader(settingsPath))
                {
                    return (List<T>?)serializer.Deserialize(reader);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"{typeof(T).ToString().Substring(typeof(T).ToString().LastIndexOf('.') + 1)} Err file not found:{ex}");
                return new List<T>();
            }
        }

        private static void SaveToXMLFile<T>(List<T> objectModel, string settingsPath)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<T>));
                var xmlNamespaces = new XmlSerializerNamespaces();
                xmlNamespaces.Add("", "");


                using (var writer = new StreamWriter(settingsPath))
                {
                    serializer.Serialize(writer, objectModel, xmlNamespaces);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"{typeof(T).ToString().Substring(typeof(T).ToString().LastIndexOf('.') + 1)} Err:{ex}");
                return;
            }
        }

        #endregion
    }
}
