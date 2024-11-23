DROP SCHEMA public CASCADE;
CREATE SCHEMA public;


CREATE TABLE players (
                         id SERIAL PRIMARY KEY,
                         name VARCHAR(255) NOT NULL,
                         email VARCHAR(255) UNIQUE NOT NULL,
                         phone VARCHAR(50),
                         balance DECIMAL(10, 2) DEFAULT 0.0, -- In-game currency balance
                         annual_fee_paid BOOLEAN DEFAULT FALSE, -- 0 = Not Paid, 1 = Paid
                         created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP

);
CREATE TABLE admins (
                        id SERIAL PRIMARY KEY,
                        name VARCHAR(255) NOT NULL,
                        email VARCHAR(255) UNIQUE NOT NULL,
                        phone VARCHAR(50),
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);


CREATE TABLE games (
                       id SERIAL PRIMARY KEY,
                       admin_id INT NOT NULL,
                       start_date DATE NOT NULL DEFAULT CURRENT_DATE, -- Start of the week
                       end_date DATE, -- When the admin submits the winning sequence
                       is_closed BOOLEAN DEFAULT FALSE, -- Indicates if the game is over
                       winning_numbers INTEGER[] CHECK (array_length(winning_numbers, 1) = 3), -- Stores the winning numbers as an array (3 numbers)
                       total_revenue DECIMAL(10, 2) NOT NULL DEFAULT 0.0, -- Total weekly revenue from board sales
                       prize_pool DECIMAL(10, 2) NOT NULL DEFAULT 0.0, -- 70% of the revenue goes to the prize pool
                       rollover_amount DECIMAL(10, 2) DEFAULT 0.0, -- Prize amount carried over if no winners
                       created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    
);


CREATE TABLE boards (
                        id SERIAL PRIMARY KEY,
                        player_id INT NOT NULL,
                        game_id INT NOT NULL,
                        numbers VARCHAR(50) NOT NULL, -- The player's guessed numbers (e.g., '3,7,8')
                        autoplay BOOLEAN DEFAULT FALSE, -- 0 = No, 1 = Yes (continue for next games)
                        fields_count INT NOT NULL, -- Number of fields selected (5-8)
                        cost DECIMAL(10, 2) NOT NULL, -- Based on fields count (20/40/80/160 DKK)
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        CONSTRAINT boards_player_id_fkey FOREIGN KEY (player_id) REFERENCES players (id),
                        CONSTRAINT boards_game_id_fkey FOREIGN KEY (game_id) REFERENCES games (id)
);


CREATE TABLE transactions (
                              id SERIAL PRIMARY KEY,
                              player_id INT NOT NULL,
                              amount DECIMAL(10, 2) NOT NULL, -- Positive for deposits, negative for purchases
                              transaction_type VARCHAR(50) NOT NULL, -- 'Deposit' or 'Purchase'
                              mobilepay_number VARCHAR(100) NOT NULL, -- Only for deposits
                              created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                              CONSTRAINT transactions_player_id_fkey FOREIGN KEY (player_id) REFERENCES players (id)
);


CREATE TABLE winners (
                         id SERIAL PRIMARY KEY,
                         game_id INT NOT NULL,
                         player_id INT NOT NULL,
                         board_id INT NOT NULL,
                         winning_amount DECIMAL(10, 2) NOT NULL, -- Share of the prize pool
                         created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                         CONSTRAINT winners_game_id_fkey FOREIGN KEY (game_id) REFERENCES games (id),
                         CONSTRAINT winners_player_id_fkey FOREIGN KEY (player_id) REFERENCES players (id),
                         CONSTRAINT winners_board_id_fkey FOREIGN KEY (board_id) REFERENCES boards (id)
);



CREATE INDEX "IX_boards_player_id" ON boards (player_id);
CREATE INDEX "IX_boards_game_id" ON boards (game_id);
CREATE INDEX "IX_transactions_player_id" ON transactions (player_id);
CREATE INDEX "IX_winners_board_id" ON winners (board_id);
CREATE INDEX "IX_winners_game_id" ON winners (game_id);
CREATE UNIQUE INDEX unique_board_numbers ON boards (player_id, game_id, numbers);