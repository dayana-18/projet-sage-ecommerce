﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using projet_sage_ecommerce.WebReference;
using SAGE_Client_WS;
using System.Net;

namespace projet_sage_ecommerce.Models
{
    public class CAdxModel
    {
        private CAdxClientBasicAuth client = new CAdxClientBasicAuth(); // Le web service 
        private CAdxCallContext context = new CAdxCallContext(); // Le contexte d’appel
        private CAdxResultXml resultat; // Le résultat de l’appel
        private CAdxParamKeyValue[] param = new CAdxParamKeyValue[2];
        private string json; // entrée
        private string wsalias;
        private int nbParam;

        public CAdxClientBasicAuth Client { get => client; set => client = value; }
        public CAdxCallContext Context { get => context; set => context = value; }
        public CAdxResultXml Resultat { get => resultat; set => resultat = value; }
        public CAdxParamKeyValue[] Param { get => param; set => param = value; }
        public string Json { get => json; set => json = value; }
        public string WsAlias { get => wsalias; set => wsalias = value; }
        public int NbParam { get => nbParam; set => nbParam = value; }

        public CAdxModel()
        {
            Client.Url = Properties.Settings.Default.projet_sage_ecommerce_WebReference_CAdxWebServiceXmlCCService;
            NetworkCredential netCredential = new NetworkCredential(Properties.Settings.Default.user, Properties.Settings.Default.password);
            Client.Credentials = netCredential;
            Client.PreAuthenticate = true;

            Context.codeLang = Properties.Settings.Default.code_lang;
            Context.poolAlias = Properties.Settings.Default.pool_alias;
            Context.requestConfig = Properties.Settings.Default.request_config;
        }

        public void run()
        {
            Json = Json.Replace("'", "\"");
            this.resultat = Client.run(Context, wsalias, json);
        }

        public void readObject()
        {
            this.resultat = Client.read(Context, wsalias, param);
        }
        
        public void modifyObject()
        {
            Json = Json.Replace("'", "\"");

            this.resultat = Client.modify(Context, wsalias, param, Json);
        }

        public void save()
        {
            Json = Json.Replace("'", "\"");

            this.resultat = Client.save(Context, wsalias, json);
        }
        public void delete()
        {
            this.resultat = Client.delete(Context, wsalias, param);
        }
    }
}