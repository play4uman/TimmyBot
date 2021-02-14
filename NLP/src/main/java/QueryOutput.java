import java.util.List;

public class QueryOutput {
    private List<String> BM25Tokens;
    private List<String> BERTTokens;

    public List<String> getBM25Tokens() {
        return BM25Tokens;
    }

    public void setBM25Tokens(List<String> BM25Tokens) {
        this.BM25Tokens = BM25Tokens;
    }

    public List<String> getBERTTokens() {
        return BERTTokens;
    }

    public void setBERTTokens(List<String> BERTTokens) {
        this.BERTTokens = BERTTokens;
    }
}
