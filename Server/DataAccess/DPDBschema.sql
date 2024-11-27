-- Drop and recreate schema
DROP SCHEMA public CASCADE;
CREATE SCHEMA public;

-- Enable UUID generation extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";


-- Players table
CREATE TABLE players (
                         id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                         name VARCHAR(255) NOT NULL,
                         email VARCHAR(255) UNIQUE NOT NULL,
                         phone VARCHAR(50),
                         balance DECIMAL(10, 2) DEFAULT 0.0, -- In-game currency balance
                         annual_fee_paid BOOLEAN DEFAULT FALSE, -- True if annual fee is paid
                         created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Admins table
CREATE TABLE admins (
                        id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                        name VARCHAR(255) NOT NULL,
                        email VARCHAR(255) UNIQUE NOT NULL,
                        phone VARCHAR(50),
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Games table
CREATE TABLE games (
                       id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                       admin_id UUID NOT NULL,
                       start_date DATE NOT NULL DEFAULT CURRENT_DATE, -- Start of the week
                       end_date DATE, -- When the admin submits the winning sequence
                       is_closed BOOLEAN DEFAULT FALSE, -- Indicates if the game is over
                       winning_numbers INTEGER[] CHECK (array_length(winning_numbers, 1) = 3), -- Winning numbers array
                       total_revenue DECIMAL(10, 2) NOT NULL DEFAULT 0.0, -- Total weekly revenue
                       prize_pool DECIMAL(10, 2) NOT NULL DEFAULT 0.0, -- 70% of revenue for prizes
                       rollover_amount DECIMAL(10, 2) DEFAULT 0.0, -- Prize rollover if no winners
                       created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                       CONSTRAINT games_admin_id_fkey FOREIGN KEY (admin_id) REFERENCES admins (id)
);

-- Boards table
CREATE TABLE boards (
                        id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                        player_id UUID NOT NULL,
                        game_id UUID NOT NULL,
                        numbers VARCHAR(50) NOT NULL, -- The player's guessed numbers (e.g., '3,7,8')
                        autoplay BOOLEAN DEFAULT FALSE, -- 0 = No, 1 = Yes (continue for next games)
                        fields_count INT NOT NULL, -- Number of fields selected (5-8)
                        cost DECIMAL(10, 2) NOT NULL, -- Based on fields count (20/40/80/160 DKK)
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        CONSTRAINT boards_player_id_fkey FOREIGN KEY (player_id) REFERENCES players (id),
                        CONSTRAINT boards_game_id_fkey FOREIGN KEY (game_id) REFERENCES games (id),
                        CHECK (
                            (fields_count = 5 AND cost = 20) OR
                            (fields_count = 6 AND cost = 40) OR
                            (fields_count = 7 AND cost = 80) OR
                            (fields_count = 8 AND cost = 160)
                            )
);

-- Transactions table
CREATE TABLE transactions (
                              id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                              player_id UUID NOT NULL,
                              amount DECIMAL(10, 2) NOT NULL, -- Positive for deposits, negative for purchases
                              transaction_type VARCHAR(50) NOT NULL, -- 'Deposit' or 'Purchase'
                              mobilepay_number VARCHAR(100), -- Only for deposits
                              created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                              CONSTRAINT transactions_player_id_fkey FOREIGN KEY (player_id) REFERENCES players (id)
);

-- Winners table
CREATE TABLE winners (
                         id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                         game_id UUID NOT NULL,
                         player_id UUID NOT NULL,
                         board_id UUID NOT NULL,
                         winning_amount DECIMAL(10, 2) NOT NULL, -- Share of the prize pool
                         created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                         CONSTRAINT winners_game_id_fkey FOREIGN KEY (game_id) REFERENCES games (id),
                         CONSTRAINT winners_player_id_fkey FOREIGN KEY (player_id) REFERENCES players (id),
                         CONSTRAINT winners_board_id_fkey FOREIGN KEY (board_id) REFERENCES boards (id)
);

-- Indexes for optimization
CREATE INDEX "IX_boards_player_id" ON boards (player_id);
CREATE INDEX "IX_boards_game_id" ON boards (game_id);
CREATE INDEX "IX_transactions_player_id" ON transactions (player_id);
CREATE INDEX "IX_winners_board_id" ON winners (board_id);
CREATE INDEX "IX_winners_game_id" ON winners (game_id);

-- Ensure unique numbers per board in a game
CREATE UNIQUE INDEX unique_board_numbers ON boards (player_id, game_id, numbers);
