db = db.getSiblingDB('local');

db.createCollection('urls');

print('Collection "urls" created in database "local"');
