BEGIN;

-- Vendors
INSERT INTO "Vendors" ("Id", "Name", "Email")
VALUES
  ('vendor-acme', 'Acme Hardware', 'sales@acme-hardware.test'),
  ('vendor-nova', 'Nova Industrial', 'hello@nova-industrial.test'),
  ('vendor-orbit', 'Orbit Systems', 'contact@orbit-systems.test')
ON CONFLICT ("Id") DO UPDATE
SET
  "Name" = EXCLUDED."Name",
  "Email" = EXCLUDED."Email";

-- Users
-- PasswordHash values are dummy placeholders for test data only.
INSERT INTO "Users" ("Id", "Username", "Email", "PasswordHash", "CreatedAt")
VALUES
  ('user-demo-alice', 'alice_demo', 'alice_demo@example.com', 'dummy_hash_alice', NOW()),
  ('user-demo-bob', 'bob_demo', 'bob_demo@example.com', 'dummy_hash_bob', NOW()),
  ('user-demo-cara', 'cara_demo', 'cara_demo@example.com', 'dummy_hash_cara', NOW())
ON CONFLICT ("Username") DO UPDATE
SET
  "Email" = EXCLUDED."Email",
  "PasswordHash" = EXCLUDED."PasswordHash";

-- Assets (18 rows for pagination testing)
INSERT INTO "Assets" ("Id", "Name", "Description", "Price", "CreatedAt", "UserId")
VALUES
  ('asset-001', '4K Camera Kit', 'Mirrorless camera body with two lenses', 1899.00, '2026-04-01T09:00:00.000Z', 'user-demo-alice'),
  ('asset-002', 'Studio Lighting Pack', 'Three-point LED lighting setup', 549.00, '2026-04-01T09:10:00.000Z', 'user-demo-alice'),
  ('asset-003', 'Audio Recorder Pro', 'Field recorder with dual XLR inputs', 329.00, '2026-04-01T09:20:00.000Z', 'user-demo-bob'),
  ('asset-004', 'Drone Survey Unit', 'Aerial drone for site inspection', 1299.00, '2026-04-01T09:30:00.000Z', 'user-demo-bob'),
  ('asset-005', 'Rugged Tablet', 'Water-resistant industrial tablet', 999.00, '2026-04-01T09:40:00.000Z', 'user-demo-cara'),
  ('asset-006', 'Network Switch 24', 'Managed 24-port gigabit switch', 449.00, '2026-04-01T09:50:00.000Z', 'user-demo-cara'),
  ('asset-007', 'Thermal Scanner', 'Handheld thermal imaging scanner', 799.00, '2026-04-01T10:00:00.000Z', 'user-demo-alice'),
  ('asset-008', 'GPS Tracker Set', 'Set of five long-life GPS trackers', 625.00, '2026-04-01T10:10:00.000Z', 'user-demo-bob'),
  ('asset-009', 'Laptop Fleet A', 'Batch of 10 developer laptops', 12499.00, '2026-04-01T10:20:00.000Z', 'user-demo-cara'),
  ('asset-010', 'Server Rack Kit', '42U rack with cooling accessories', 2199.00, '2026-04-01T10:30:00.000Z', 'user-demo-alice'),
  ('asset-011', '3D Printer XL', 'Large volume prototyping printer', 3499.00, '2026-04-01T10:40:00.000Z', 'user-demo-bob'),
  ('asset-012', 'UPS Backup 3000', '3kVA uninterrupted power supply', 1199.00, '2026-04-01T10:50:00.000Z', 'user-demo-cara'),
  ('asset-013', 'Conference Display 86', '86-inch 4K meeting room display', 2799.00, '2026-04-01T11:00:00.000Z', 'user-demo-alice'),
  ('asset-014', 'Barcode Scanner Cart', 'Mobile scanner cart with cradle', 699.00, '2026-04-01T11:10:00.000Z', 'user-demo-bob'),
  ('asset-015', 'VR Training Headset', 'Standalone VR unit for training sims', 499.00, '2026-04-01T11:20:00.000Z', 'user-demo-cara'),
  ('asset-016', 'NAS Storage 48TB', 'Network storage array with RAID', 2699.00, '2026-04-01T11:30:00.000Z', 'user-demo-alice'),
  ('asset-017', 'Label Printer Duo', 'Dual-roll industrial label printer', 389.00, '2026-04-01T11:40:00.000Z', 'user-demo-bob'),
  ('asset-018', 'Portable Projector', 'Compact projector for field demos', 459.00, '2026-04-01T11:50:00.000Z', 'user-demo-cara')
ON CONFLICT ("Id") DO UPDATE
SET
  "Name" = EXCLUDED."Name",
  "Description" = EXCLUDED."Description",
  "Price" = EXCLUDED."Price",
  "CreatedAt" = EXCLUDED."CreatedAt",
  "UserId" = EXCLUDED."UserId";

COMMIT;

-- Quick verification
SELECT 'Users' AS "Table", COUNT(*) AS "Rows" FROM "Users"
UNION ALL
SELECT 'Vendors' AS "Table", COUNT(*) AS "Rows" FROM "Vendors"
UNION ALL
SELECT 'Assets' AS "Table", COUNT(*) AS "Rows" FROM "Assets";
