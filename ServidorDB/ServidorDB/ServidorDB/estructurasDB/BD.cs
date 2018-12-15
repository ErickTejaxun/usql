﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace ServidorDB.estructurasDB
{
    class BD
    {
        public String nombre;
        public String path;
        public List<Tabla> tablas;
        public List<Objeto> objetos;
        public List<Procedimiento> procedimientos;        

        public BD(String nombre, String path)
        {
            this.nombre = nombre;
            this.path = path;
            this.tablas = new List<Tabla>();
            this.objetos = new List<Objeto>();
            this.procedimientos = new List<Procedimiento>();
        }


        //< @param orden: 1 ascendente, 0 descendente>
        //< @param campoOrdenacion:Nombre del campo para ordenar>
        //< @param Campos: campos a seleccionar>
        //< @param listaTabla: lista de nombres de las tablas>
        //< @param raiz : Raiz de la condición a cumplirse>
        public String seleccionar(List<String> listaCampos, 
            List<String> listaTablas, ParseTreeNode raiz, String campoOrdenacion, int orden)
        {            
            String data = "\n";
            List<tupla> cartesianoTemporal = new List<tupla>();
            List<tupla> cartesiano = new List<tupla>();
            /*Primero realizamos el producto cartesiano de las tablas involucradas*/
            foreach (String ntab in listaTablas)
            {
                cartesianoTemporal = productoCartesiano(cartesianoTemporal, buscarTabla(ntab) , ntab);                
            }
            /*Verificamos las condiciones para filtrar resultados*/
            foreach (tupla tp in cartesianoTemporal)
            {
                if (comprobarCondicion(tp, raiz))
                {
                    cartesiano.Add(tp);
                }
            }

            /*Elegimos las celdas que se solicitan. Las demás se descartan*/
            cartesiano = filtrarResultados(listaCampos, cartesiano);

            /**/
            cartesiano = ordenarResultados(cartesiano, campoOrdenacion, orden);

            #region imprimir resultado
            foreach (campo cp in cartesiano[0].campos)
            {
                if (data.Equals("\n"))
                {
                    data = cp.id;
                }
                else
                {
                    data = data + "," + cp.id;
                }
            }
            data = data + "\n";
            foreach (tupla tpm in cartesiano)
            {
                bool flag = false;
                foreach (campo cmp in tpm.campos)
                {
                    if (!flag)
                    {
                        data = data + cmp.valor.ToString();
                        flag = true;
                    }
                    else
                    {
                        data = data + "," + cmp.valor.ToString();
                    }                    
                }
                data = data + "\n";
            }
            #endregion

            return data;
        }

        public List<tupla> ordenarResultados(List<tupla> listaCampos, String campoOrdenacion, int orden)
        {
            List<tupla> listaOrdenada = new List<tupla>();
            if (orden>2)
            {
                return listaCampos;
            }
            int posicion = 0;
            foreach (campo cp in listaCampos[0].campos)
            {
                if(cp.id.ToLower().Equals(campoOrdenacion.ToLower()))
                {
                    break;
                }
                posicion++;
            }
            if (orden == 1)
            {
                var ordenado =
                    from campos in listaCampos
                    //orderby campos.campos[posicion] ascending
                    select new { campos };
                foreach (var data in ordenado)
                {
                    tupla tpm = new tupla();
                    foreach (campo cmp in data.campos.campos)
                    {
                        tpm.addCampo(cmp);
                    }
                    listaOrdenada.Add(tpm);
                }
            }
            if (orden == 0)
            {
                var ordenado =
                    from campos in listaCampos
                    orderby campos.campos[posicion] descending
                    select campos;
                foreach (var data in ordenado)
                {
                    tupla tpm = new tupla();
                    foreach (campo cmp in data.campos)
                    {
                        tpm.addCampo(cmp);
                    }
                    listaOrdenada.Add(tpm);
                }
            }

            return listaOrdenada;
        }


        public Boolean comprobarCondicion(tupla tup, ParseTreeNode raiz)
        {
            if (raiz==null)
            {
                return true;
            }


            return true;
        }

        public List<tupla> filtrarResultados(List<String> listaCampos, List<tupla> cartesiano)
        {
            List<tupla> listaFiltrada = new List<tupla>();
            if(listaCampos.Count == 0)
            {
                return cartesiano;
            }
            foreach (tupla tp in cartesiano)
            {
                tupla nuevaTupla = new tupla();
                foreach (campo cp in tp.campos)
                {
                    foreach (String campoBuscado in listaCampos)
                    {
                        if (campoBuscado.ToLower().Equals(cp.id.ToLower()))// Es un campo buscado
                        {
                            nuevaTupla.addCampo(cp);
                        }
                    }
                }
                listaFiltrada.Add(nuevaTupla);
            }
            return listaFiltrada;
        }
        public List<tupla> productoCartesiano(List<tupla> tab1, List<tupla> tab2 , String nombre)
        {
            List<tupla> tablaCar = new List<tupla>();
            if (tab1.Count >0 && tab2.Count > 0)
            {
                foreach (tupla tp in tab2)
                {
                    foreach (campo cp in tp.campos)
                    {
                        cp.id = nombre + "." + cp.id;
                    }
                }

                var cartesiano = from campo1 in tab1
                                 from campo2 in tab2
                                 select new { campo1, campo2 };
                foreach (var data in cartesiano)
                {
                    tupla tpm = new tupla();
                    foreach (campo cmp in data.campo1.campos)
                    {                        
                        tpm.addCampo(cmp);
                    }
                    foreach (campo cmp in data.campo2.campos)
                    {                        
                        tpm.addCampo(cmp); 
                    }
                    tablaCar.Add(tpm);
                }
            }
            else
            {
                foreach (tupla tp in tab2)
                {
                    foreach (campo cp in tp.campos)
                    {
                        cp.id = nombre +"."+ cp.id;
                    }
                }
                return tab2;
            }
            return tablaCar;
        }

        public List<tupla> buscarTabla(String id)
        {
            foreach (Tabla tab in tablas)
            {
                if (tab.nombre.Equals(id))
                {
                    return tab.tuplas;
                }
            }
            return null;
        }
    }

}