# import redis
# from redis.commands.json.path import Path
# import redis.commands.search.aggregation as aggregations
# import redis.commands.search.reducers as reducers
# from redis.commands.search.field import TextField, NumericField, TagField
# from redis.commands.search.index_definition import IndexDefinition, IndexType
# from redis.commands.search.query import Query
# import redis.exceptions

# r = redis.Redis(decode_responses=True)

# # schema = (
# #     TextField("$.name", as_name="name"),
# #     TagField("$.city", as_name="city"),
# #     NumericField("$.age", as_name="age")
# # )

# # indexCreated = r.ft("idx:users").create_index(
# #     schema,
# #     definition=IndexDefinition(
# #         prefix=["user:"], index_type=IndexType.JSON
# #     )
# # )

# user1Set = r.json().set("user:1", Path.root_path(), user1)
# user2Set = r.json().set("user:2", Path.root_path(), user2)
# user3Set = r.json().set("user:3", Path.root_path(), user3)
