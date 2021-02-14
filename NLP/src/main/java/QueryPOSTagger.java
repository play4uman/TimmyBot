import opennlp.tools.postag.POSModel;
import opennlp.tools.postag.POSTagger;
import opennlp.tools.postag.POSTaggerME;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;

public class QueryPOSTagger {
    private String tags[];

    public QueryPOSTagger(String query) throws IOException {
        QueryTokenizer queryTokenizer = new QueryTokenizer(query);
        POSTaggerME posTaggerMe = CreateTags(query);
        setTags(posTaggerMe.tag(queryTokenizer.getTokens()));
    }

    public String[] getTags() {
        return tags;
    }

    public void setTags(String[] tags) {
        this.tags = tags;
    }

    private POSModel LoadModel() throws IOException {
        try (InputStream posModelIn = getClass().getResourceAsStream("en-pos-maxent.bin")) {
            POSModel posModel = new POSModel(posModelIn);

            return posModel;
        }
    }

    private POSTaggerME CreateTags(String query) throws IOException {
        POSModel posModel = LoadModel();
        POSTaggerME posTagger = new POSTaggerME(posModel);

        return  posTagger;
    }
}
