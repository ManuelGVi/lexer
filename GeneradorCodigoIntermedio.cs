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
            string resultado = ProcesarReturn(nodo);
            if (!string.IsNullOrEmpty(resultado)){
                cuadruplos.Add(new Cuadruplo("Return", resultado, null,null));
            }
        }
       
        else if (nodo.Valor == "Bloque")
        {
            foreach (var hijo in nodo.Hijos)
            {
                ProcesarNodo(hijo);
            }
        }
            else if (nodo.Valor == "Operador: =")
    {
        return ProcesarAsignacion(nodo);
    }
    else if (nodo.Valor.StartsWith("Operador: "))
    {
        return ProcesarOperacion(nodo);
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

    // Procesar la condición (primer hijo)
    string condicion = ProcesarNodo(nodo.Hijos[0]);
    string bloquin= ProcesarNodo(nodo.Hijos[1]);
    // Crear etiquetas para los saltos
    string etiquetaFin = $"L{contadorEtiqueta++}";
    if (nodo.Hijos[1].Valor == "Sentencia else"){
         // Cuádruplo: IF_FALSE condición, salta a etiquetaFin si es falso
    cuadruplos.Add(new Cuadruplo("IF_FALSE", condicion, null, etiquetaFin));

    // Procesar el bloque verdadero (Sentencia Return u otra)
    ProcesarNodo(nodo.Hijos[1].Hijos[0]);  // El bloque del If (cuando la condición es verdadera)

    // EtiquetaFin
    cuadruplos.Add(new Cuadruplo("LABEL", null, null, etiquetaFin));
    ProcesarNodo(nodo.Hijos[1].Hijos[1]);
    }
else{
    // Cuádruplo: IF_FALSE condición, salta a etiquetaFin si es falso
    cuadruplos.Add(new Cuadruplo("IF_FALSE", condicion, null, etiquetaFin));

    // Procesar el bloque verdadero (Sentencia Return u otra)
    ProcesarNodo(nodo.Hijos[1]);  // El bloque del If (cuando la condición es verdadera)

    // EtiquetaFin
    cuadruplos.Add(new Cuadruplo("LABEL", null, null, etiquetaFin));
}
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
    private string ProcesarOperacion(Nodo nodo)
    {
        string operador = nodo.Valor.Substring(10);

        string operando1 = ProcesarNodo(nodo.Hijos[0]);
        string operando2 = ProcesarNodo(nodo.Hijos[1]);

        string resultado = $"t{contadorTemporal++}";
        cuadruplos.Add(new Cuadruplo(operador, operando1, operando2, resultado));

        return resultado;
    }
    private string ProcesarReturn(Nodo nodo)
{
    string operando = ProcesarNodo(nodo.Hijos[0]);

    // Si el hijo es un operador, procesa la operación
    if (nodo.Hijos[0].Valor.StartsWith("Operador: "))
    {
        string operador = nodo.Hijos[0].Valor.Substring(10);
        string operando1 = ProcesarNodo(nodo.Hijos[0].Hijos[0]);
        string operando2 = ProcesarNodo(nodo.Hijos[0].Hijos[1]);

        string resultado = $"t{contadorTemporal++}";
        cuadruplos.Add(new Cuadruplo(operador, operando1, operando2, resultado));
        return resultado;
    }
    else
    {
        // Si no es un operador, es un operando directo
        return operando;
    }
}
private string ProcesarAsignacion(Nodo nodo)
{
    string variable = nodo.Hijos[0].Valor.Split(": ")[1];  // Nombre de la variable (e.g., y)
    string valor = ProcesarOperacion(nodo.Hijos[1]);            // Procesar la operación o valor asignado (e.g., y + 1)

    cuadruplos.Add(new Cuadruplo("=", valor, null, variable));  // Cuádruplo de asignación
    return variable;  // Retorna la variable asignada
}


}