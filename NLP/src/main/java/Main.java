import com.google.gson.Gson;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

public class Main {
    public static void main(String[] args) throws IOException {

        QueryParser queryParser = new QueryParser(args[0]);

        QueryOutput queryOutput = new QueryOutput();
        queryOutput.setBERTTokens(queryParser.GetBERTQuery());
        queryOutput.setBM25Tokens(queryParser.GetBM25Query());

        Gson serializer = new Gson();
        String serializedOutput = serializer.toJson(queryOutput);
        System.out.println(serializedOutput);
    }
}
