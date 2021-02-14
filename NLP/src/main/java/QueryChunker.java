import opennlp.tools.chunker.ChunkerME;
import opennlp.tools.chunker.ChunkerModel;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;

public class QueryChunker {

    private String chunks[];
    public QueryChunker(String query) throws IOException {
        QueryTokenizer queryTokenizer = new QueryTokenizer(query);
        QueryPOSTagger queryPOSTagger = new QueryPOSTagger(query);

        ChunkerME chunker = LoadModel();
        setChunks(chunker.chunk(queryTokenizer.getTokens(), queryPOSTagger.getTags()));
    }

    public String[] getChunks() {
        return chunks;
    }

    public void setChunks(String[] chunks) {
        this.chunks = chunks;
    }

    private ChunkerME LoadModel() throws IOException {
        try (InputStream modelIn = getClass().getResourceAsStream("en-chunker.bin")) {
            ChunkerModel chunkerModel = new ChunkerModel(modelIn);
            ChunkerME chunker = new ChunkerME(chunkerModel);

            return chunker;
        }
    }
}
