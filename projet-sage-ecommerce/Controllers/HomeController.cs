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

            JArray jsonArray3 = (JArray)json.GetValue("GRP3");

            e = 0;
            foreach (JObject jsonObject in jsonArray3)
            {
                string des = (string)jsonObject.SelectToken("YDESCRIPTION");

                ViewData["description" + e.ToString()] = des; // client.Resultat.resultXml;
                e++;
            }

            return View("Catalogue", client);
        }

        public ActionResult SuiviCommande()
        {
            CAdxModel c = new CAdxModel();
            return View(c);   
        }

        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult SuiviCommande(CAdxModel c, string status = "")
        {
            c = new CAdxModel();

            c.WsAlias = "WSYCOMERP";
            c.Param[0] = new CAdxParamKeyValue();

            c.Param[0].key = "SOHNUM";

            if(Session["numcommandeSession"] == null)
            {
                c.Param[0].value = Request.Form["order-num"];
                Session["numcommandeSession"] = c.Param[0].value;
            }
            else if (Request.Form["order-num"] != (string)Session["numcommandeSession"] && Request.Form["order-num"] != null /*!= String.Empty*/)
            {
                c.Param[0].value = Request.Form["order-num"];
                Session["numcommandeSession"] = c.Param[0].value;
            }
            else
            {
                c.Param[0].value = (string)Session["numcommandeSession"];
                ViewData["numcommande"] = "blank";
            }
                       
            c.readObject();

            JObject json = JObject.Parse(c.Resultat.resultXml);
            // Zone principale 
            ViewData["sitedevente"] = json.GetValue("SOH0_1").SelectToken("SALFCY"); // Site de vente
            ViewData["numcommande"] = json.GetValue("SOH0_1").SelectToken("SOHNUM"); // Numéro de la commande
            DateTime date = new DateTime(Int32.Parse(json.GetValue("SOH0_1").SelectToken("ORDDAT").ToString().Substring(0,4)), Int32.Parse(json.GetValue("SOH0_1").SelectToken("ORDDAT").ToString().Substring(4, 2)), Int32.Parse(json.GetValue("SOH0_1").SelectToken("ORDDAT").ToString().Substring(6, 2)));
            ViewData["datecommande"] = date.ToString().Substring(0, 10); // Date de la commande
            ViewData["codeclient"] = json.GetValue("SOH0_1").SelectToken("BPCORD"); // Num client
            // État de la commande
            ViewData["etatcommande"] = json.GetValue("SOH1_5").SelectToken("ORDSTA_LBL"); // État de la commande
            ViewData["facturation"] = json.GetValue("SOH1_5").SelectToken("INVSTA_LBL"); // État de la facturation
            // Fournisseur
            ViewData["fournisseur"] = json.GetValue("SOH2_1").SelectToken("STOFCY"); // Fournisseur
            ViewData["fournisseurnom"] = json.GetValue("SOH2_1").SelectToken("ZSTOFCY"); // Nom du fournisseur
            ViewData["priorite"] = json.GetValue("SOH2_1").SelectToken("DLVPIO_LBL"); // Priorité de la livraison

            //DEVIS 
            ViewData["numdevis"] = json.GetValue("SOH3_3").SelectToken("SQHNUM"); // Fournisseur
            //Articles
            JArray jsonArray = (JArray)json.GetValue("SOH4_1");//Get list of items

            int e = 0;
            foreach (JObject jsonObject in jsonArray)
            {
                CAdxModel tempC = new CAdxModel();
                tempC.WsAlias = "WSYITM";
                tempC.Param[0] = new CAdxParamKeyValue();
                tempC.Param[0].key = "ITMREF";
                tempC.Param[0].value = (string)jsonObject.SelectToken("ITMREF");
                tempC.readObject();
                JObject jsonTemp = JObject.Parse(tempC.Resultat.resultXml);
                ViewData["blob" + e.ToString()] = jsonTemp.GetValue("ITM1_7").SelectToken("IMG");

                ViewData["ITMREF" + e.ToString()] = jsonObject.SelectToken("ITMREF"); // id de l'article
                ViewData["ITMDES" + e.ToString()] = jsonObject.SelectToken("ITMDES"); // designation
                ViewData["QTY" + e.ToString()] = jsonObject.SelectToken("QTY"); // quantité
                ViewData["GROPRI" + e.ToString()] = jsonObject.SelectToken("GROPRI"); // prix unitaire
                ViewData["LINORDNOT" + e.ToString()] = jsonObject.SelectToken("LINORDNOT"); // total ligne
                e++;
            }
            ViewData["length"] = e;
            //Prix
            ViewData["prixttht"] = json.GetValue("SOH4_4").SelectToken("ORDINVNOT"); // Prix total HT
            ViewData["prixttTTC"] = json.GetValue("SOH4_4").SelectToken("ORDINVATI"); // Prix total TTC

            JArray jsonArray1 = (JArray)json.GetValue("SOH3_5");//Get list of items
            JObject jobj = (JObject)jsonArray1[0];
            JObject jobj1 = (JObject)jsonArray1[2];
            ViewData["remise"] = jobj.GetValue("INVDTAAMT"); // Remise %
            ViewData["assurance"] = jobj1.GetValue("INVDTAAMT"); // Assurance %

            //Livraison | Information transporteur
            ViewData["transporteurnum"] = json.GetValue("SOH2_3").SelectToken("BPTNUM"); // Id du transporteur
            ViewData["transporteurnom"] = json.GetValue("SOH2_3").SelectToken("ZBPTNUM"); // Nom du transporteur
            //ViewData["modedelivraison"] = json.GetValue("SOH2_3").SelectToken("MDL");
            ViewData["modedelivraisonnom"] = json.GetValue("SOH2_3").SelectToken("ZMDL");
            ViewData["livraisonnum"] = json.GetValue("SOH2_4").SelectToken("LASDLVNUM"); //Numéro de la livraison
            DateTime date1 = new DateTime(Int32.Parse(json.GetValue("SOH2_4").SelectToken("LASDLVDAT").ToString().Substring(0, 4)), Int32.Parse(json.GetValue("SOH2_4").SelectToken("LASDLVDAT").ToString().Substring(4, 2)), Int32.Parse(json.GetValue("SOH2_4").SelectToken("LASDLVDAT").ToString().Substring(6, 2)));
            ViewData["livraisondate"] = date1.ToString().Substring(0, 10); // Date de la livraison
            
            ViewData["douane"] = json.GetValue("SOH2_3").SelectToken("ZEECICT"); //Douane
            ViewData["tournee"] = json.GetValue("SOH2_3").SelectToken("DRN_LBL"); //tournee
            ViewData["partielle"] = json.GetValue("SOH2_6").SelectToken("DME_LBL"); //partielle
            DateTime date2 = new DateTime(Int32.Parse(json.GetValue("SOH2_2").SelectToken("DEMDLVDAT").ToString().Substring(0, 4)), Int32.Parse(json.GetValue("SOH2_2").SelectToken("DEMDLVDAT").ToString().Substring(4, 2)), Int32.Parse(json.GetValue("SOH2_2").SelectToken("DEMDLVDAT").ToString().Substring(6, 2)));
            ViewData["livraisondatedemandee"] = date2.ToString().Substring(0, 10); // Date de la livraison demandée
            
            DateTime date3 = new DateTime(Int32.Parse(json.GetValue("SOH2_2").SelectToken("SHIDAT").ToString().Substring(0, 4)), Int32.Parse(json.GetValue("SOH2_2").SelectToken("SHIDAT").ToString().Substring(4, 2)), Int32.Parse(json.GetValue("SOH2_2").SelectToken("SHIDAT").ToString().Substring(6, 2)));
            ViewData["livraisondateexpedition"] = date3.ToString().Substring(0, 10); // Date d'expedition

            ViewData["delai"] = json.GetValue("SOH2_2").SelectToken("DAYLTI"); // délai prévu
            ViewData["heureprevu"] = json.GetValue("SOH2_2").SelectToken("DEMDLVHOU").ToString().Substring(0, 2) + ":" + json.GetValue("SOH2_2").SelectToken("DEMDLVHOU").ToString().Substring(2, 2); // délai prévu
            // Adresse 
            ViewData["adpays"] = json.GetValue("ADB2_1").SelectToken("CRYNAM"); //Pays
            JArray jsonArray2 = (JArray)json.GetValue("ADB2_1").SelectToken("BPAADDLIG");//Adresse
            ViewData["rue"] = jsonArray2[0].ToString();
            ViewData["codepostal"] = json.GetValue("ADB2_1").SelectToken("POSCOD"); //Code postal
            ViewData["ville"] = json.GetValue("ADB2_1").SelectToken("CTY"); //ville
            ViewData["statutdelivry"] = json.GetValue("SOH1_5").SelectToken("DLVSTA_LBL"); // statut de la livraison
                                                                                           //Facturation
            ViewData["conditionpaiement"] = json.GetValue("SOH3_3").SelectToken("PTE"); // Condition paiement
            ViewData["nomconditionpaiement"] = json.GetValue("SOH3_3").SelectToken("ZPTE"); // Nom condition paiement
            ViewData["numerodudevis"] = json.GetValue("SOH3_3").SelectToken("SQHNUM"); // Numéro du devis

            DateTime date4 = new DateTime(Int32.Parse(json.GetValue("SOH3_2").SelectToken("VCRINVCNDDAT").ToString().Substring(0, 4)), Int32.Parse(json.GetValue("SOH3_2").SelectToken("VCRINVCNDDAT").ToString().Substring(4, 2)), Int32.Parse(json.GetValue("SOH3_2").SelectToken("VCRINVCNDDAT").ToString().Substring(6, 2)));
            ViewData["dateecheance"] = date4.ToString().Substring(0, 10); // Date de l'échéance

            json = JObject.Parse(c.Resultat.resultXml);

            return View("SuiviCommande");
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
            ViewData["recherche"] = Request["nom_article_recherche"].ToUpper();

            JObject json = JObject.Parse(client.Resultat.resultXml);
            JArray jsonArray = (JArray)json.GetValue("GRP1");
            JArray jsonArrayDesc = (JArray)json.GetValue("GRP3");

            int e = 0;
            foreach (JObject jsonObject in jsonArray) {
                ViewData["nom" + e.ToString()] = jsonObject.SelectToken("ITMREF"); // client.Resultat.resultXml;
                ViewData["des" + e.ToString()] = jsonObject.SelectToken("ITMDES1"); // client.Resultat.resultXml;
                ViewData["blob" + e.ToString()] = jsonObject.SelectToken("BLOB"); // client.Resultat.resultXml;
                ViewData["prix" + e.ToString()] = jsonObject.SelectToken("BASPRI"); // client.Resultat.resultXml;
                ViewData["quantite" + e.ToString()] = jsonObject.SelectToken("QTYPCU"); // client.Resultat.resultXml;
                
                e++;
            }
            e = 0;
            foreach (JObject jsonObject in jsonArrayDesc) {
                ViewData["description" + e.ToString()] = jsonObject.SelectToken("YDESCRIPTION");
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

        //--------------------------------------------------devis---------------------------------------------------

        public ActionResult Devis()
        {
            CAdxModel client = new CAdxModel();

            client.WsAlias = "WSYDEVIS"; //WJWSDEVIS
            //client.Json = "{''}";

            //'ITMREF': '" + id + @"',
            client.Json = @"{
                              'SQH0_1': {
                                'SALFCY': 'FR015',
                                'SQHTYP': 'SQN',
                                'QUODAT': '20210409',
                                'BPCORD': 'YYCLF1'
                              },
                              'SQH1_2': {
                                'STOFCY': 'FR014',
                                'EECICT': ''
                              },
                              'SQH1_4': {
                                'VACBPR': 'FRA',
                                'CUR': 'EUR',
                                'PRITYP': '1'
                              },
                              'SQH3_1': {
                                'PTE': 'CH30NETEOM'
                              },
                              'SQH2_1': [
                                {
                                  'ITMREF': 'HHMANG',
                                  'QTY': '3'
                                }
                              ]
                            }";

            //String json2 = client.Json.Replace("'", "\"");
            //client.Json.Replace("'", "\"");

            client.save();

            //nblig nbre de lignes tableau

            JObject json = JObject.Parse(client.Resultat.resultXml);
            JArray jsonArray = (JArray)json.GetValue("SQH2_1");

            ViewData["sitevente"] = json.GetValue("SQH0_1").SelectToken("SALFCY"); // client.Resultat.resultXml;
            ViewData["typedevis"] = json.GetValue("SQH0_1").SelectToken("SQHTYP");
            ViewData["date"] = json.GetValue("SQH0_1").SelectToken("QUODAT");
            ViewData["client"] = json.GetValue("SQH0_1").SelectToken("BPCORD");
            ViewData["adr_cli"] = json.GetValue("SQH1_1").SelectToken("BPAADD");
            ViewData["siteexpedition"] = json.GetValue("SQH1_2").SelectToken("STOFCY");

            //tableau article
            int e = 0;
            foreach (JObject jsonObject in jsonArray)
            {
                ViewData["idarticle"] = jsonObject.SelectToken("ITMREF"); // article ITMREF et qte QTY
                ViewData["des"] = jsonObject.SelectToken("ITMDES");
                ViewData["prix"] = jsonObject.SelectToken("NETPRI");
                ViewData["qte"] = jsonObject.SelectToken("QTY");
                e++;
            }
            ViewData["length"] = e;

            return View("Devis", client);
        }

        public ActionResult Commande()
        {
            CAdxModel client = new CAdxModel();
            ViewBag.Message = "Your Commande page.";

            return View();
        }

        public ActionResult Error404() {
            Response.StatusCode = 404;
            return View("Error404");
        }

    }
}
