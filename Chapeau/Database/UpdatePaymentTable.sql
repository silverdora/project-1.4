-- First, create a new column with the correct type
ALTER TABLE Payment
ADD paymentType_new VARCHAR(20);

-- Update the new column with values from the old column
UPDATE Payment
SET paymentType_new = paymentType;

-- Drop the old column
ALTER TABLE Payment
DROP COLUMN paymentType;

-- Rename the new column to paymentType
EXEC sp_rename 'Payment.paymentType_new', 'paymentType', 'COLUMN';

-- Add a check constraint to ensure only valid payment types are used
ALTER TABLE Payment
ADD CONSTRAINT CK_PaymentType CHECK (paymentType IN ('Cash', 'DebitCard', 'CreditCard')); 