﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft;
using projet_sage_ecommerce.Models;
using projet_sage_ecommerce.WebReference;
using SAGE_Client_WS;
namespace projet_sage_ecommerce.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            CAdxModel client = new CAdxModel();

            client.WsAlias = "WSYTESTITM";
            client.Json = "{}";

            client.run();

            for (int i = 0; i < client.Resultat.messages.Length; i++) // Boucle pour récupérer les messages
            {
                if (client.Resultat.messages[i].type.Equals("1")) Console.WriteLine("INFORMATION : ");
                if (client.Resultat.messages[i].type.Equals("2")) Console.WriteLine("AVERTISSEMENT : ");
                if (client.Resultat.messages[i].type.Equals("3")) Console.WriteLine("ERREUR : ");
                Console.WriteLine(client.Resultat.messages[i].message);
            }

            JObject json = JObject.Parse(client.Resultat.resultXml);

            JArray jsonArray = (JArray)json.GetValue("GRP1");
            int e = 0;
            foreach (JObject jsonObject in jsonArray)
            {
                ViewData["nom" + e.ToString()] = jsonObject.SelectToken("ITMREF"); // client.Resultat.resultXml;
                ViewData["des" + e.ToString()] = jsonObject.SelectToken("ITMDES1"); // client.Resultat.resultXml;
                ViewData["des2" + e.ToString()] = jsonObject.SelectToken("ITMDES2"); // client.Resultat.resultXml;
                ViewData["blob" + e.ToString()] = jsonObject.SelectToken("BLOB"); // client.Resultat.resultXml;
                ViewData["prix" + e.ToString()] = jsonObject.SelectToken("BASPRI"); // client.Resultat.resultXml;
                ViewData["quantite" + e.ToString()] = jsonObject.SelectToken("QTYPCU"); // client.Resultat.resultXml;
                e++;
            }
            
            ViewData["length"] = e;

            JArray jsonArray3 = (JArray)json.GetValue("GRP3");

            e = 0;
            foreach (JObject jsonObject in jsonArray3)
            {
                string des = (string)jsonObject.SelectToken("YDESCRIPTION");

                ViewData["description" + e.ToString()] = des; // client.Resultat.resultXml;
                e++;
            }
            
            return View("Index", client);
        }

        public ActionResult Catalogue()
        {
            CAdxModel client = new CAdxModel();
            ViewBag.Title = "Catalogue";
            ViewBag.Message = "Tous les produits du Dada Shop";

            client.WsAlias = "WSYTESTITM";
            client.Json = "{}";

            client.run();

            string r = "";
            for (int i = 0; i < client.Resultat.messages.Length; i++) // Boucle pour récupérer les messages
            {
                if (client.Resultat.messages[i].type.Equals("1")) Console.WriteLine("INFORMATION : ");
                if (client.Resultat.messages[i].type.Equals("2")) Console.WriteLine("AVERTISSEMENT : ");
                if (client.Resultat.messages[i].type.Equals("3")) Console.WriteLine("ERREUR : ");
                Console.WriteLine(client.Resultat.messages[i].message);
            }
            JObject json = JObject.Parse(client.Resultat.resultXml);
            JArray jsonArray = (JArray)json.GetValue("GRP1");
            int e = 0;
            foreach (JObject jsonObject in jsonArray)
            {
                ViewData["nom" + e.ToString()] = jsonObject.SelectToken("ITMREF"); // client.Resultat.resultXml;
                ViewData["des" + e.ToString()] = jsonObject.SelectToken("ITMDES1"); // client.Resultat.resultXml;
                ViewData["des2" + e.ToString()] = jsonObject.SelectToken("ITMDES2"); // client.Resultat.resultXml;
                ViewData["blob" + e.ToString()] = jsonObject.SelectToken("BLOB"); // client.Resultat.resultXml;
                ViewData["prix" + e.ToString()] = jsonObject.SelectToken("BASPRI"); // client.Resultat.resultXml;
                ViewData["quantite" + e.ToString()] = jsonObject.SelectToken("QTYPCU"); // client.Resultat.resultXml;
                e++;
            }
            ViewData["length"] = e;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Title = "A propos";
            ViewBag.Message = "Projet ERP - Site e-commerce Sage X3 & C# ASP.NET";           
             
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Run()
        {
            CAdxModel client = new CAdxModel();
            ViewBag.Message = "Your run page.";

            return View(client);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Run(CAdxModel c)
        {
            CAdxModel client = new CAdxModel();

            client.WsAlias = Request.Form["wsname_param"];
            client.Json = Request.Form["entreexml"];

            client.run();

            for (int i = 0; i < client.Resultat.messages.Length; i++) // Boucle pour récupérer les messages
            {
                if (client.Resultat.messages[i].type.Equals("1")) Console.WriteLine("INFORMATION : ");
                if (client.Resultat.messages[i].type.Equals("2")) Console.WriteLine("AVERTISSEMENT : ");
                if (client.Resultat.messages[i].type.Equals("3")) Console.WriteLine("ERREUR : ");
                Console.WriteLine(client.Resultat.messages[i].message);
            }
            JObject json = JObject.Parse(client.Resultat.resultXml);
            JArray jsonArray = (JArray)json.GetValue("GRP1");

            int e = 0; 
            foreach(JObject jsonObject in jsonArray)
            {
                ViewData["nom" + e.ToString()] = jsonObject.SelectToken("ITMREF"); // client.Resultat.resultXml;
                ViewData["des" + e.ToString()] = jsonObject.SelectToken("ITMDES1"); // client.Resultat.resultXml;
                ViewData["blob" + e.ToString()] = jsonObject.SelectToken("BLOB"); // client.Resultat.resultXml;
                ViewData["prix" + e.ToString()] = jsonObject.SelectToken("BASPRI"); // client.Resultat.resultXml;
                ViewData["quantite" + e.ToString()] = jsonObject.SelectToken("QTYPCU"); // client.Resultat.resultXml;
                e++;
            }
            ViewData["length"] = e;
            return View("Run", client);
        }

        public ActionResult Item(String id)
        {
            CAdxModel c = new CAdxModel();

            c.WsAlias = "WSYITM";
            c.Param[0] = new CAdxParamKeyValue();

            c.Param[0].key = "ITMREF";
            c.Param[0].value = id;

            c.readObject();
            
            for (int i = 0; i < c.Resultat.messages.Length; i++) // Boucle pour récupérer les messages
            {
                if (c.Resultat.messages[i].type.Equals("1")) Console.WriteLine("INFORMATION : ");
                if (c.Resultat.messages[i].type.Equals("2")) Console.WriteLine("AVERTISSEMENT : ");
                if (c.Resultat.messages[i].type.Equals("3")) Console.WriteLine("ERREUR : ");
                Console.WriteLine(c.Resultat.messages[i].message);
            }

            JObject json = JObject.Parse(c.Resultat.resultXml);

            ViewData["nom"] = json.GetValue("ITM0_1").SelectToken("ITMREF"); // client.Resultat.resultXml;
            ViewData["des"] = json.GetValue("ITM0_1").SelectToken("DES1AXX"); // client.Resultat.resultXml;
            ViewData["blob"] = json.GetValue("ITM1_7").SelectToken("IMG"); // client.Resultat.resultXml;
            ViewData["prix"] = json.GetValue("ITS_3").SelectToken("BASPRI"); // client.Resultat.resultXml;
            ViewData["devise"] = json.GetValue("ITS_3").SelectToken("CUR"); // client.Resultat.resultXml;
            ViewData["description"] = json.GetValue("ITM0_1").SelectToken("YDESCRIPTION"); // client.Resultat.resultXml;

            c.Param[1] = new CAdxParamKeyValue();
            c.Param[1].key = "STOFCY";
            c.Param[1].value = "FR014";

            c.WsAlias = "WSSTOCK";
            
            c.readObject();

            json = JObject.Parse(c.Resultat.resultXml);

            ViewData["quantite"] = json.GetValue("ITF8_1").SelectToken("PHYSTO"); // client.Resultat.resultXml;

            return View("Item", c);
        }

        public ActionResult Read()
        {
            CAdxModel client = new CAdxModel();
            ViewBag.Message = "Your read page.";

            return View(client);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Read(CAdxModel c)
        {
            CAdxModel client = new CAdxModel();

            client.WsAlias = "WSYTESTITM";
            client.Json = "{}";

            client.run();

            for (int i = 0; i < client.Resultat.messages.Length; i++) // Boucle pour récupérer les messages
            {
                if (client.Resultat.messages[i].type.Equals("1")) Console.WriteLine("INFORMATION : ");
                if (client.Resultat.messages[i].type.Equals("2")) Console.WriteLine("AVERTISSEMENT : ");
                if (client.Resultat.messages[i].type.Equals("3")) Console.WriteLine("ERREUR : ");
                Console.WriteLine(client.Resultat.messages[i].message);
            }
            ViewData["recherche"] = Request.Form["nom_article_recherche"];

            JObject json = JObject.Parse(client.Resultat.resultXml);
            JArray jsonArray = (JArray)json.GetValue("GRP1");

            int e = 0;
            foreach (JObject jsonObject in jsonArray) {
                ViewData["nom" + e.ToString()] = jsonObject.SelectToken("ITMREF"); // client.Resultat.resultXml;
                ViewData["des" + e.ToString()] = jsonObject.SelectToken("ITMDES1"); // client.Resultat.resultXml;
                ViewData["blob" + e.ToString()] = jsonObject.SelectToken("BLOB"); // client.Resultat.resultXml;
                ViewData["prix" + e.ToString()] = jsonObject.SelectToken("BASPRI"); // client.Resultat.resultXml;
                ViewData["quantite" + e.ToString()] = jsonObject.SelectToken("QTYPCU"); // client.Resultat.resultXml;
                e++;
            }
            ViewData["length"] = e;
            return View("Read", client);
        }

        public ActionResult Modify()
        {
            CAdxModel client = new CAdxModel();
            ViewBag.Message = "Your read page.";

            return View(client);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Modify(CAdxModel c)
        {
            CAdxModel client = new CAdxModel();

            client.WsAlias = Request.Form["wsname_param"];
           // client.Param = Request.Form["key_param"];
            //client.Value = Request.Form["value_param"];
            client.Json = Request.Form["xml_entry"];

            client.modifyObject();
            string r = "";
            for (int i = 0; i < client.Resultat.messages.Length; i++) // Boucle pour récupérer les messages
            {
                if (client.Resultat.messages[i].type.Equals("1")) Console.WriteLine("INFORMATION : ");
                if (client.Resultat.messages[i].type.Equals("2")) Console.WriteLine("AVERTISSEMENT : ");
                if (client.Resultat.messages[i].type.Equals("3")) Console.WriteLine("ERREUR : ");
                Console.WriteLine(client.Resultat.messages[i].message);
            }

            ViewData["message"] = client.Resultat.resultXml;

            //JObject json = JObject.Parse(client.Resultat.resultXml);
            //Console.WriteLine(json.GetValue("ITM0_1"));

            return View("Modify", client);
        }

        public ActionResult Devis()
        {
            CAdxModel client = new CAdxModel();
            ViewBag.Message = "Your Devis page.";

            return View();
        }
        
        public ActionResult Commande()
        {
            CAdxModel client = new CAdxModel();
            ViewBag.Message = "Your Commande page.";

            return View();
        }

    }
}
