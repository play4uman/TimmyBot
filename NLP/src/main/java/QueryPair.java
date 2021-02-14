public class QueryPair {
    private String word;
    private String chunk;

    public QueryPair(String word, String chunk) {
        setWord(word);
        setChunk(chunk);
    }

    public String getWord() {
        return word;
    }

    public void setWord(String word) {
        this.word = word;
    }

    public String getChunk() {
        return chunk;
    }

    public void setChunk(String chunk) {
        this.chunk = chunk;
    }
}
