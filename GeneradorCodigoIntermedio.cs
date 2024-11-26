using System;
using System.Collections.Generic;

public class GeneradorCodigoIntermedio
{
    private List<Cuadruplo> cuadruplos;
    private int contadorTemporal;
    private int contadorEtiqueta;

    public GeneradorCodigoIntermedio()
    {
        cuadruplos = new List<Cuadruplo>();
        contadorTemporal = 1;
        contadorEtiqueta = 1;
    }

    public List<Cuadruplo> GenerarCodigoIntermedio(Nodo nodo)
    {
        ProcesarNodo(nodo);
        return cuadruplos;
    }

    private string ProcesarNodo(Nodo nodo)
    {
        if (nodo == null)
        {
            Console.WriteLine("Nodo nulo. Terminando procesamiento.");
            return null;
        }

        Console.WriteLine($"Procesando nodo: {nodo.Valor}");

        if (nodo.Valor == "Programa")
        {
            foreach (var hijo in nodo.Hijos)
            {
                ProcesarNodo(hijo);
            }
        }
        else if (nodo.Valor == "Sentencia If")
        {
            ProcesarIf(nodo);
        }
        else if (nodo.Valor == "Sentencia While")
        {
            ProcesarWhile(nodo);  // Aquí agregamos el procesamiento del While
        }
        else if (nodo.Valor == "Sentencia Return")
        {
            string operador = nodo.Hijos[0].Valor; // Este es el operador como '+' o '-'
            string operando1 = ProcesarNodo(nodo.Hijos[1]);
            string operando2 = ProcesarNodo(nodo.Hijos[2]);

            string resultado = $"t{contadorTemporal++}";
            cuadruplos.Add(new Cuadruplo(operador, operando1, operando2, resultado));
            cuadruplos.Add(new Cuadruplo("RETURN", resultado, null, null));
        }
       
        else if (nodo.Valor == "Bloque")
        {
            foreach (var hijo in nodo.Hijos)
            {
                ProcesarNodo(hijo);
            }
        }
        else if (nodo.Valor.StartsWith("Operador: "))
        {
            // Manejo de operadores como antes
            string operador = nodo.Valor.Substring(10);
            string operando1 = ProcesarNodo(nodo.Hijos[0]);
            string operando2 = ProcesarNodo(nodo.Hijos[1]);

            string resultado = $"t{contadorTemporal++}";
            cuadruplos.Add(new Cuadruplo(operador, operando1, operando2, resultado));
            return resultado;
        }
        else if (nodo.Valor.StartsWith("Identificador: ") || nodo.Valor.StartsWith("Operando: "))
        {
            return nodo.Valor.Split(": ")[1];
        }

        return null;
    }

    private void ProcesarIf(Nodo nodo)
{
    Console.WriteLine("Procesando Sentencia If...");

    // Procesar la condición (operador y operandos)
    string condicion = ProcesarNodo(nodo.Hijos[0]);

    // Crear etiquetas para los saltos
    string etiquetaElse = $"L{contadorEtiqueta++}";
    string etiquetaFin = $"L{contadorEtiqueta++}";

    // Cuádruplo: IF_FALSE condición, salta a etiquetaElse
    cuadruplos.Add(new Cuadruplo("IF_FALSE", condicion, null, etiquetaElse));

    // Procesar el bloque del if (verdadero)
    ProcesarNodo(nodo.Hijos[1]);

    // Cuádruplo: GOTO etiquetaFin
    cuadruplos.Add(new Cuadruplo("GOTO", null, null, etiquetaFin));

    // EtiquetaElse
    cuadruplos.Add(new Cuadruplo("LABEL", null, null, etiquetaElse));

    // Si el segundo hijo es un nodo "Sentencia else", procesamos el bloque 'else'
    // Ya que en tu árbol binario solo hay dos hijos, el segundo hijo es o el bloque "if" o el bloque "else"
    if (nodo.Hijos[1].Valor == "Sentencia else")
    {
        ProcesarElse(nodo); // Procesar el bloque 'else'
    }

    // EtiquetaFin
    cuadruplos.Add(new Cuadruplo("LABEL", null, null, etiquetaFin));
}


    private void ProcesarWhile(Nodo nodo)
    {
        Console.WriteLine("Procesando Sentencia While...");

        // Procesar la condición (operador y operandos)
        string condicion = ProcesarNodo(nodo.Hijos[0]);

        // Crear etiquetas para los saltos
        string etiquetaInicio = $"L{contadorEtiqueta++}";
        string etiquetaFin = $"L{contadorEtiqueta++}";

        // EtiquetaInicio
        cuadruplos.Add(new Cuadruplo("LABEL", null, null, etiquetaInicio));

        // Cuádruplo: IF_FALSE condición, salta a etiquetaFin (si la condición es falsa)
        cuadruplos.Add(new Cuadruplo("IF_FALSE", condicion, null, etiquetaFin));

        // Procesar el bloque del while (cuerpo del while)
        ProcesarNodo(nodo.Hijos[1]);

        // Cuádruplo: GOTO etiquetaInicio (salta al inicio para volver a comprobar la condición)
        cuadruplos.Add(new Cuadruplo("GOTO", null, null, etiquetaInicio));

        // EtiquetaFin
        cuadruplos.Add(new Cuadruplo("LABEL", null, null, etiquetaFin));
    }

    private string ProcesarAsignacion(Nodo nodo)
    {
        string identificador = ProcesarNodo(nodo.Hijos[0]);
        string expresion = ProcesarNodo(nodo.Hijos[1]);

        cuadruplos.Add(new Cuadruplo("=", expresion, null, identificador));
        return identificador;
    }

    private string ProcesarOperacion(Nodo nodo)
    {
        string operador = nodo.Valor.Substring(10);

        string operando1 = ProcesarNodo(nodo.Hijos[0]);
        string operando2 = ProcesarNodo(nodo.Hijos[1]);

        string resultado = $"t{contadorTemporal++}";
        cuadruplos.Add(new Cuadruplo(operador, operando1, operando2, resultado));

        return resultado;
    }

    private void ProcesarElse(Nodo nodo)
    {
        Console.WriteLine("Procesando Sentencia Else...");

        // Verificar si el bloque 'else' tiene hijos (instrucciones dentro del else)
        if (nodo.Hijos.Count > 0)
        {
            foreach (var hijo in nodo.Hijos)
            {
                ProcesarNodo(hijo); // Procesar el bloque dentro del 'else'
            }
        }
        else
        {
            Console.WriteLine("El bloque 'else' no tiene instrucciones.");
        }
    }
}
