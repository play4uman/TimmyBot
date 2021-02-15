import java.io.*;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashSet;
import java.util.List;
import java.util.stream.Collectors;

public class QueryParser {

    private ArrayList<QueryPair> chunckedTokens;
    private QueryTokenizer queryTokenizer;
    private QueryChunker queryChunker;
    private QueryPOSTagger queryPOSTagger;

    private String toBeVerbs[] = {"is", "are", "was", "were"};

    public QueryParser(String query) throws IOException {
        queryTokenizer = new QueryTokenizer(query);
        queryPOSTagger = new QueryPOSTagger(query);
        queryChunker = new QueryChunker(query);
        generateChunkedTokens();
    }

    public ArrayList<QueryPair> getChunkedTokens() {
        return chunckedTokens;
    }

    private void generateChunkedTokens() {
        chunckedTokens = new ArrayList<>();
        for(int i = 0; i < queryTokenizer.getTokens().length; i++) {
            QueryPair pair = new QueryPair(queryTokenizer.getTokens()[i], queryChunker.getChunks()[i], queryPOSTagger.getTags()[i]);
            chunckedTokens.add(pair);
        }
    }

    public List<String> GetBERTQuery() throws IOException {
        StringBuilder ngram = null;
        ArrayList<String> result = new ArrayList<>();
        for (int i = 0; i < chunckedTokens.size() - 1; i++) {
           if(chunckedTokens.get(i).getChunk().startsWith("B")) {
               ngram = new StringBuilder(chunckedTokens.get(i).getWord());

               for(int j = i + 1; j < chunckedTokens.size() && chunckedTokens.get(j).getChunk().startsWith("I"); j++) {
                   ngram.append(" " + chunckedTokens.get(j).getWord());
               }
               result.add(ngram.toString());
           }
        }
        return result;
    }

    public List<String> GetBM25Query() throws IOException {
        List<String> chunkedTokens = GetBERTQuery();
        try (InputStream stopWords = getClass().getResourceAsStream("common-english-words.txt")) {
            BufferedReader reader = new BufferedReader(new InputStreamReader(stopWords));
            String firstLine = reader.readLine();
            String stopWordsAsTokens[] = firstLine.split(",");
            HashSet<String> contents = Arrays.stream(stopWordsAsTokens)
                    .collect(Collectors.toCollection(HashSet::new));
            List<String> result = chunkedTokens.stream()
                    .filter(t -> !contents.contains(t.toLowerCase()))
                    .map(t ->{
                        String tokens[] = t.split("\\s+");
                        if(tokens.length >= 2 && Arrays.stream(toBeVerbs).anyMatch(tokens[0]::equalsIgnoreCase)){
                            String temp = tokens[0];
                            tokens[0] = tokens[1];
                            tokens[1] = temp;
                        }
                        return String.format("%s %s", tokens[0], tokens[1]);
                    })
                    .collect(Collectors.toList());
            return result;
        }


    }
}
