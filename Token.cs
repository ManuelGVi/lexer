public enum TipoToken
{
    PalabraClave,
    Identificador,
    OperadorL,
    OperadorM,
    Numero,
    Delimitador,
    Comentario,
    Desconocido
}
public class Token
{
    public TipoToken Tipo { get; set; }
    public string Valor { get; set; }
    public int Linea { get; set; }
    public int Columna { get; set; }

    public Token(TipoToken tipo, string valor, int linea, int columna)
    {
        Tipo = tipo;
        Valor = valor;
        Linea = linea;
        Columna = columna;
    }
}
