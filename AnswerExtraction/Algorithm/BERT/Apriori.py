import numpy as np
import pandas as pd
file = "https://files.grouplens.org/datasets/movielens/ml-100k/u.data"
data = pd.read_csv(file,delimiter='\t',header=None,names = ['UserID','MovieID','Rating','Datetime'])
data["Datetime"] = pd.to_datetime(data['Datetime'],unit='s')
data[:5]

# determine if a person recommends a movie
data["Favor"] = data["Rating"] > 3
data[10:15]

data[data["UserID"] == 1][:5]


# Select the top 200 users
data_200 = data[data['UserID'].isin(range(200))]
# only the favorable reviews
favor_data = data_200[data_200["Favor"] == 1]
favor_data[:5]

# Put the user's commented movie into a collection
 # frozenset Freeze collection
favor_reviews_by_users = dict((k, frozenset(v.values)) for k, v in favor_data.groupby("UserID")["MovieID"])
print(favor_reviews_by_users[1])
print(len(favor_reviews_by_users))

#  Get the recommended number
num_favor_by_movie = data_200[["MovieID","Favor"]].groupby("MovieID").sum()
num_favor_by_movie.sort_values("Favor",ascending=False)[:5]


from collections import defaultdict
import sys
def find_frequent_itemsets(favor_reviews_by_users,k_itemsets,min_support):
    counts = defaultdict(int)
    # iterate over all of the users and their reviews
    for user,review in favor_reviews_by_users.items():
        # see whether itemset is a subset of the reviews or not
        for itemset in k_itemsets:
            if itemset.issubset(review):
                for other_reviewed_movie in review-itemset:
                    current_superset = itemset|frozenset((other_reviewed_movie,))
                    counts[current_superset] += 1
    return dict([(itemset,frequence) for itemset,frequence in counts.items() if frequence >= min_support])


frequent_itemsets={}
min_support = 50
 # frequent item set of length 1
frequent_itemsets[1] = dict((frozenset((movie_id,)),row['Favor']) for movie_id,row in num_favor_by_movie.iterrows() if row['Favor'] > min_support)
print("There are {0} movies with more than {1} favor reviews.".format(len(frequent_itemsets[1]),min_support))
for k in range(2,20):
         #Generate k frequent itemsets by k-1 frequent itemsets
    cur_frequent_itemsets = find_frequent_itemsets(favor_reviews_by_users,frequent_itemsets[k-1],min_support)
    if len(cur_frequent_itemsets) == 0:
        print("Did not any frequent itemsets of length {}".format(k))
        sys.stdout.flush()
        break
    else:
        print("I found {} frequent itemsets of length {}".format(len(cur_frequent_itemsets),k))
        sys.stdout.flush()
        frequent_itemsets[k] = cur_frequent_itemsets
 # Frequent itemsets of length 1 do not need
del frequent_itemsets[1]
print("Found a total of {} frequent itemsets.".format(sum(len(frequent_item) for frequent_item in frequent_itemsets.values())))

# Now, we create the association rules.First ,they can ben the candidate rules until tested
candidate_rules = []

for itemset_length,itemset_counts in frequent_itemsets.items():
    for itemset in itemset_counts.keys():
                 # Take one of the items as a conclusion, others as a premise
        for conclusion in itemset:
            premise = itemset - set((conclusion,))
            candidate_rules.append((premise,conclusion))
print("There are {} candidate rules in total.".format(len(candidate_rules)))
print(candidate_rules[:5])


# Now,we compute the confidence of each of these rules.
correct_counts = defaultdict(int)
incorrect_counts = defaultdict(int)

for user,reviews in favor_reviews_by_users.items():
    for candidate_rule in candidate_rules:
        premise,conclusion = candidate_rule
        if premise.issubset(reviews):
            if conclusion in reviews:
                correct_counts[candidate_rule] += 1
            else:
                incorrect_counts[candidate_rule] += 1
rule_confidence = {candidate_rule: correct_counts[candidate_rule] / float(correct_counts[candidate_rule] + incorrect_counts[candidate_rule])
                      for candidate_rule in candidate_rules}


# set the min_confidence
min_confidence = 0.9
# filter out the poor rules
rule_confidence = {rule: confidence for rule,confidence in rule_confidence.items() if confidence > min_confidence}
print(len(rule_confidence))


from operator import itemgetter
sort_confidence = sorted(rule_confidence.items(),key=itemgetter(1),reverse = True)
for index in range(0,5):
    print("Rule #{0}:".format(index+1))
    premise,conclusion = sort_confidence[index][0]
    print("Rule: If a person recommends {0} they will also recommend {1}".format(premise,conclusion))
    print("- Confidence: {0:.1f}%".format(sort_confidence[index][1]))


# Even better, we can get the movie titles themselves from the dataset
movie_name_file = "https://files.grouplens.org/datasets/movielens/ml-100k/u.item"
movie_name_data = pd.read_csv(movie_name_file,delimiter="|",header=None,encoding="mac-roman")
movie_name_data.columns = ["MovieID", "Title", "Release Date", "Video Release", "IMDB", "<UNK>", "Action", "Adventure",
                           "Animation", "Children's", "Comedy", "Crime", "Documentary", "Drama", "Fantasy", "Film-Noir",
                           "Horror", "Musical", "Mystery", "Romance", "Sci-Fi", "Thriller", "War", "Western"]

def get_movie_name(movie_id):
    title_object = movie_name_data[movie_name_data["MovieID"] == movie_id]["Title"]
    title = title_object.values[0]
    return title
get_movie_name(1)

for index in range(0,5):
    print("Rule #{0}:".format(index+1))
    premise,conclusion = sort_confidence[index][0]
    premise_name = ", ".join(get_movie_name(idx) for idx in premise)
    conclusion_name = get_movie_name(conclusion)
    print("Rule: If a person recommends {0} they will also recommend {1}".format(premise_name,conclusion_name))
    print("- Confidence: {0:.1f}%".format(sort_confidence[index][1]))
