-- public.movies definition

-- Drop table

-- DROP TABLE public.movies;

CREATE TABLE IF NOT EXISTS public.movies (
	title varchar(200) NOT NULL,
	releasedate date NOT NULL,
	category varchar(150) NOT NULL,
	id uuid NOT NULL,
	CONSTRAINT movies_pk PRIMARY KEY (id)
);

-- public.users definition

-- Drop table

-- DROP TABLE public.users;

CREATE TABLE IF NOT EXISTS public.users (
	"name" varchar NOT NULL,
	email varchar NOT NULL,
	passwordhash varchar NOT NULL,
	id uuid NOT NULL,
	createddate date NOT NULL,
	CONSTRAINT users_pk PRIMARY KEY (id),
	CONSTRAINT users_unique UNIQUE (email)
);