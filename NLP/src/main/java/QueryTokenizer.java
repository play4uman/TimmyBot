import opennlp.tools.tokenize.TokenizerME;
import opennlp.tools.tokenize.TokenizerModel;

import java.io.*;

public class QueryTokenizer {

    private String tokens[];
    public QueryTokenizer(String query) throws IOException {

        TokenizerME tokenizer = LoadModel();
        setTokens(tokenizer.tokenize(query));
    }

    public String[] getTokens() {
        return tokens;
    }

    public void setTokens(String[] tokens) {
        this.tokens = tokens;
    }

    private TokenizerME LoadModel() throws IOException {
        try (InputStream modelIn = getClass().getResourceAsStream("en-token.bin")) {
            TokenizerModel model = new TokenizerModel(modelIn);
            TokenizerME tokenizer = new TokenizerME(model);

            return  tokenizer;
        }
    }
}
