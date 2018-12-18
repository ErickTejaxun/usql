﻿using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServidorDB.estructurasDB;
using ServidorDB.EjecucionUsql;
using ServidorDB;

namespace ServidorBDD.EjecucionUsql
{


    public class Interprete
    {

        //nuevas
        public static TablaSimbolo tablaGlobal;
        public static TablaSimbolo tabla;
        public Logica opL;
        public Interprete()
        {
            tabla = new TablaSimbolo();
            tablaGlobal = new TablaSimbolo();
        }

        public Resultado ejecutar(ParseTreeNode raiz)
        {
            foreach (ParseTreeNode hijo in raiz.ChildNodes)
            {
                String tipoAccion = hijo.Term.Name;
                switch (tipoAccion)
                {
                    case "DECLARACION"://jose
                        Declaracion declaracion = new Declaracion();
                        Boolean estado = declaracion.declaracion(hijo);
                        break;
                    case "SELECCIONAR":
                        Form1.sistemaArchivos.realizarConsulta(hijo);
                        break;                    
                    case "USAR":
                        Form1.sistemaArchivos.setBaseActual(hijo.ChildNodes[0]);
                        break;
                    case "IMPRIMIR":
                        opL = new Logica();
                        Form1.Mensajes.Add(opL.operar(hijo.ChildNodes[0]).valor + "");
                        break;
                    case "ACTUALIZAR":
                        //Form1.sistemaArchivos.actualizacion(hijo);
                        break;
                    case "INSERTAR":
                        Form1.sistemaArchivos.insertar(hijo);
                        break;
                    case "CREAROBJETO"://jose
                        String nombreObjeto = hijo.ChildNodes[0].Token.Text;
                        ParseTreeNode atributos = hijo.ChildNodes[1];
                        Objeto objeto = new Objeto(nombreObjeto);
                        foreach (ParseTreeNode nodoAtributo in atributos.ChildNodes)
                        {

                            Atributo atributo = new Atributo(nodoAtributo.ChildNodes[0].ChildNodes[0].Token.Text, nodoAtributo.ChildNodes[1].Token.Text, null);
                            objeto.addAtributo(atributo);

                        }
                        Form1.sistemaArchivos.getBase().agregarObjeto(objeto, hijo.Span.Location.Line, hijo.Span.Location.Column);
                        break;
                    case "ASIGNAROBJ":
                        Asignacion asignacion = new Asignacion();
                        asignacion.asignar(hijo);
                        break;
                }
            }
            return new Resultado("null", null);
        }

        #region Area ejecucion

        private void agregarError(String tipo, String descripcion, int linea, int columna)
        {
            Error error = new Error(tipo, descripcion, linea, columna);
            Form1.errores.Add(error);
            Form1.Mensajes.Add(error.getMensaje());
        }

        #endregion fin area ejecucion
    }
}
