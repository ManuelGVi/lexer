using System;
using System.Collections.Generic;

public class GenerarCodigo
{
    private List<Cuadruplo> cuadruplos;

    public GenerarCodigo(List<Cuadruplo> cuadruplos)
    {
        this.cuadruplos = cuadruplos;
    }

    public void Generar()
    {
        Console.WriteLine("\nCódigo Intermedio (Pseudocódigo):");
        foreach (var cuadruplo in cuadruplos)
        {
            string codigo = FormatearCuadruplo(cuadruplo);
            Console.WriteLine(codigo);
        }
    }

    private string FormatearCuadruplo(Cuadruplo cuadruplo)
    {
        // Desglosar cuádruplos por tipo
        switch (cuadruplo.Operador)
        {
            case "LABEL":
                return $"{cuadruplo.Resultado}:";
            case "GOTO":
                return $"GOTO {cuadruplo.Resultado}";
            case "IF_FALSE":
                return $"IF NOT {cuadruplo.Operando1} GOTO {cuadruplo.Resultado}";
            case "=":
                return $"{cuadruplo.Resultado} = {cuadruplo.Operando1}";
            default:
                // Operadores binarios como +, -, *, /
                if (!string.IsNullOrEmpty(cuadruplo.Operando2))
                {
                    return $"{cuadruplo.Resultado} = {cuadruplo.Operando1} {cuadruplo.Operador} {cuadruplo.Operando2}";
                }
                break;
        }

        // Si el operador no es reconocido
        return $"// Cuádruplo desconocido: {cuadruplo.Operador}";
    }
}
