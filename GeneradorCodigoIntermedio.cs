public class GeneradorCodigoIntermedio
{
    private List<Cuadruplo> cuadruplos;
    private int contadorTemporal;

    public GeneradorCodigoIntermedio()
    {
        cuadruplos = new List<Cuadruplo>();
        contadorTemporal = 1;
    }

    public List<Cuadruplo> GenerarCodigoIntermedio(Nodo nodo)
    {
        Console.WriteLine($"Iniciando generación de cuádruplos para nodo: {nodo.Valor}");
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
        // Procesar cada hijo del nodo "Programa"
        foreach (var hijo in nodo.Hijos)
        {
            ProcesarNodo(hijo);
        }
        return null; // No hay un resultado directo para "Programa"
    }
    else if (nodo.Valor.StartsWith("Operador: "))
    {
        string operador = nodo.Valor.Substring(10);

        Console.WriteLine($"Operador encontrado: {operador}");

        string operando1 = ProcesarNodo(nodo.Hijos[0]);
        string operando2 = ProcesarNodo(nodo.Hijos[1]);

        string resultado = $"t{contadorTemporal++}";
        Cuadruplo cuadruplo = new Cuadruplo(operador, operando1, operando2, resultado);
        cuadruplos.Add(cuadruplo);

        Console.WriteLine($"Generado cuádruplo: {cuadruplo}");
        return resultado;
    }
    else if (nodo.Valor.StartsWith("Identificador: ") || nodo.Valor.StartsWith("Operando: "))
    {
        string valor = nodo.Valor.Split(": ")[1];
        Console.WriteLine($"Operando encontrado: {valor}");
        return valor;
    }
   else if (nodo.Valor.StartsWith("Operador: ="))
{
    Console.WriteLine("Procesando asignación...");

    // Procesar lado izquierdo (Identificador: a)
    string identificador = ProcesarNodo(nodo.Hijos[0]);

    // Procesar lado derecho (Expresión: t1)
    string expresion = ProcesarNodo(nodo.Hijos[1]);

    // Generar cuádruplo de asignación
    Cuadruplo cuadruplo = new Cuadruplo("=", expresion, null, identificador);
    cuadruplos.Add(cuadruplo);

    Console.WriteLine($"Generado cuádruplo: {cuadruplo}");

    // Retornar identificador
    return identificador;
}


    
    else
    {
        Console.WriteLine($"Nodo desconocido: {nodo.Valor}");
    }

    return null;
}

}
