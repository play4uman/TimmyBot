public class QueryPair {
    private String word;
    private String chunk;
    private String tag;

    public QueryPair(String word, String chunk, String tag) {
        setWord(word);
        setChunk(chunk);
        setTag(tag);
    }

    public String getTag() {
        return tag;
    }

    public void setTag(String tag) {
        this.tag = tag;
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
