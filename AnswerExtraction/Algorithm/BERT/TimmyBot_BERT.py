import torch
from transformers import BertForQuestionAnswering
from transformers import BertTokenizer

# main.py
import sys

#Model
model = BertForQuestionAnswering.from_pretrained('bert-large-uncased-whole-word-masking-finetuned-squad')

#Tokenizer
tokenizer = BertTokenizer.from_pretrained('bert-large-uncased-whole-word-masking-finetuned-squad')

def get_answer(question, paragraph):
    encoding = tokenizer.encode_plus(text=question,text_pair=paragraph, add_special_tokens=True)

    inputs = encoding['input_ids']  #Token embeddings
    sentence_embedding = encoding['token_type_ids']  #Segment embeddings
    tokens = tokenizer.convert_ids_to_tokens(inputs) #input tokens

    output = model(input_ids=torch.tensor([inputs]), token_type_ids=torch.tensor([sentence_embedding]))
    sl = output.start_logits
    el = output.end_logits

    start_index = torch.argmax(sl)
    end_index = torch.argmax(el)

    answer = ' '.join(tokens[start_index:end_index+1])
    corrected_answer = ''
    for word in answer.split():
    
        #If it's a subword token
        if word[0:2] == '##':
            corrected_answer += word[2:]
        else:
            corrected_answer += ' ' + word
    print(f"ANSWER: {corrected_answer}")

def exit_command(firstIn, secondIn):
    return (firstIn =='exit') and (secondIn == 'exit')


question = ''
paragraph = ''
while not exit_command(question, paragraph) :
    question = input()
    paragraph = input()
    if not exit_command(question, paragraph):
        get_answer(question, paragraph)







