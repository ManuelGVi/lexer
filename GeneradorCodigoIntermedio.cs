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
 // Simplificar la expresión si aplica
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
        else if(nodo.Valor == "Sentencia For"){
            ProcesarFor(nodo);
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

    // Ajustar el cuádruplo para las asignaciones simples
    if (operador == "=")
    {
        cuadruplos.Add(new Cuadruplo(operador, operando2, null, operando1));
        return operando1;
    }

    string resultado = $"t{contadorTemporal++}";
    cuadruplos.Add(new Cuadruplo(operador, operando1, operando2, resultado));

    return resultado;
}

private string ProcesarReturn(Nodo nodo)
{
    // Procesamos el hijo del nodo "Return" (que es la expresión que va a ser retornada)
    string operando = ProcesarNodo(nodo.Hijos[0]);

    // Si el hijo es un operador, procesamos la operación
    if (nodo.Hijos[0].Valor.StartsWith("Operador: "))
    {
        string operador = nodo.Hijos[0].Valor.Substring(10); // Operador como "+" o "-"
        string operando1 = ProcesarNodo(nodo.Hijos[0].Hijos[0]); // Primer operando
        string operando2 = ProcesarNodo(nodo.Hijos[0].Hijos[1]); // Segundo operando

        // Verificar si ya existe un cuádruplo para esta operación antes de agregar uno nuevo
        string resultado = BuscarResultadoExistente(operador, operando1, operando2);
        if (resultado == null)
        {
            // Generamos un nuevo resultado temporal para la operación
            resultado = $"t{contadorTemporal++}";

            // Agregamos el cuadruplo de la operación
            cuadruplos.Add(new Cuadruplo(operador, operando1, operando2, resultado));
        }

        // Devolvemos el resultado de la operación para el return
        return resultado;
    }
    else
    {
        // Si no es una operación, simplemente es un operando directo
        return operando;
    }
}


private void ProcesarFor(Nodo nodo)
{
    Console.WriteLine("Procesando Sentencia For...");

    // Procesar la inicialización (primer hijo de Inicializacion)
    string inicializacion = ProcesarNodo(nodo.Hijos[0].Hijos[0].Hijos[0]); // Inicialización: Operador =

    // Procesar la condición (primer hijo de CondicionYActualizacion)
    string condicion = ProcesarNodo(nodo.Hijos[0].Hijos[1].Hijos[0].Hijos[0]); // Condición: Operador <

    // Crear etiquetas para los saltos
    string etiquetaInicio = $"L{contadorEtiqueta++}";
    string etiquetaFin = $"L{contadorEtiqueta++}";

    // EtiquetaInicio (inicio del ciclo)
    cuadruplos.Add(new Cuadruplo("LABEL", null, null, etiquetaInicio));

    // Cuádruplo: IF_FALSE condición, salta a etiquetaFin (si la condición es falsa)
    cuadruplos.Add(new Cuadruplo("IF_FALSE", condicion, null, etiquetaFin));

    // Procesar el bloque del for (cuerpo del for)
    ProcesarNodo(nodo.Hijos[1]);

    // Cuádruplo: actualización (expresión que se ejecuta después de cada ciclo)
    string actualizacion = ProcesarNodo(nodo.Hijos[0].Hijos[1].Hijos[1].Hijos[0]); // Actualización: Operador =

    // Cuádruplo: GOTO etiquetaInicio (salta al inicio para volver a comprobar la condición)
    cuadruplos.Add(new Cuadruplo("GOTO", null, null, etiquetaInicio));

    // EtiquetaFin (cuando la condición es falsa)
    cuadruplos.Add(new Cuadruplo("LABEL", null, null, etiquetaFin));
}
private string BuscarResultadoExistente(string operador, string operando1, string operando2)
{
    foreach (var cuadruplo in cuadruplos)
    {
        if (cuadruplo.Operador == operador && cuadruplo.Operando1 == operando1 && cuadruplo.Operando2 == operando2)
        {
            return cuadruplo.Resultado;
        }
    }
    return null;
}


}