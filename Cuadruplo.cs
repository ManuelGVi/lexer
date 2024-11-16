public class Cuadruplo
{
    public string Operador { get; }
    public string Operando1 { get; }
    public string Operando2 { get; }
    public string Resultado { get; }

    public Cuadruplo(string operador, string operando1, string operando2, string resultado)
    {
        Operador = operador;
        Operando1 = operando1;
        Operando2 = operando2;
        Resultado = resultado;
    }

    public override string ToString()
    {
        return $"( {Operador}, {Operando1 ?? ""}, {Operando2 ?? ""}, {Resultado} )";
    }
}
