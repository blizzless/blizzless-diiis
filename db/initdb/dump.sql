--
-- PostgreSQL database dump
--

-- Dumped from database version 14.5 (Debian 14.5-1.pgdg110+1)
-- Dumped by pg_dump version 14.4

-- Started on 2022-08-18 16:47:30 UTC

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;


CREATE DATABASE diiis WITH OWNER = postgres ENCODING = 'UTF8' LC_COLLATE = 'en_US.utf8' LC_CTYPE = 'en_US.utf8' TABLESPACE = pg_default;

CREATE DATABASE worlds WITH OWNER = postgres ENCODING = 'UTF8' LC_COLLATE = 'en_US.utf8' LC_CTYPE = 'en_US.utf8' TABLESPACE = pg_default;
--
-- TOC entry 3546 (class 1262 OID 24881)
-- Name: worlds; Type: DATABASE; Schema: -; Owner: postgres
--

\connect worlds

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 209 (class 1259 OID 25184)
-- Name: DRLG_Container; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."DRLG_Container" (
    id integer NOT NULL,
    worldsno bigint,
    rangeofscenes bigint,
    params bigint
);


ALTER TABLE public."DRLG_Container" OWNER TO postgres;

--
-- TOC entry 210 (class 1259 OID 25187)
-- Name: DRLG_Container_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."DRLG_Container_id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."DRLG_Container_id_seq" OWNER TO postgres;

--
-- TOC entry 3548 (class 0 OID 0)
-- Dependencies: 210
-- Name: DRLG_Container_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."DRLG_Container_id_seq" OWNED BY public."DRLG_Container".id;


--
-- TOC entry 211 (class 1259 OID 25188)
-- Name: DRLG_Tile; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."DRLG_Tile" (
    id integer NOT NULL,
    head_container integer,
    type integer,
    snohandle_id integer,
    snolevelarea integer,
    snomusic integer,
    snoweather integer
);


ALTER TABLE public."DRLG_Tile" OWNER TO postgres;

--
-- TOC entry 212 (class 1259 OID 25191)
-- Name: DRLG_Tile_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."DRLG_Tile_id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."DRLG_Tile_id_seq" OWNER TO postgres;

--
-- TOC entry 3549 (class 0 OID 0)
-- Dependencies: 212
-- Name: DRLG_Tile_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."DRLG_Tile_id_seq" OWNED BY public."DRLG_Tile".id;


--
-- TOC entry 213 (class 1259 OID 25192)
-- Name: account_relations; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.account_relations (
    id bigint NOT NULL,
    listowner_id bigint,
    listtarget_id bigint,
    type character varying(255) DEFAULT 'FRIEND'::character varying NOT NULL
);


ALTER TABLE public.account_relations OWNER TO postgres;

--
-- TOC entry 214 (class 1259 OID 25196)
-- Name: account_relations_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.account_relations_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.account_relations_seq OWNER TO postgres;

--
-- TOC entry 215 (class 1259 OID 25197)
-- Name: accounts; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.accounts (
    id bigint NOT NULL,
    email character varying(255),
    banned boolean DEFAULT false NOT NULL,
    salt bytea,
    passwordverifier bytea,
    saltedticket character varying(255),
    battletagname character varying(255),
    hashcode integer,
    referralcode integer,
    inviteeaccount_id bigint,
    money bigint DEFAULT 0,
    userlevel character varying(255),
    lastonline bigint,
    hasrename boolean DEFAULT false NOT NULL,
    renamecooldown bigint DEFAULT 0
);


ALTER TABLE public.accounts OWNER TO postgres;

--
-- TOC entry 216 (class 1259 OID 25206)
-- Name: accounts_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.accounts_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.accounts_seq OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 25207)
-- Name: achievements; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.achievements (
    id bigint NOT NULL,
    dbgameaccount_id bigint,
    achievementid bigint,
    completetime integer,
    ishardcore boolean DEFAULT false NOT NULL,
    quantity integer DEFAULT 0,
    criteria bytea
);


ALTER TABLE public.achievements OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 25214)
-- Name: achievements_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.achievements_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.achievements_seq OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 25215)
-- Name: collection_editions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.collection_editions (
    id bigint NOT NULL,
    setid integer,
    dbaccount_id bigint,
    claimed boolean DEFAULT false NOT NULL,
    claimedtoon_id bigint,
    claimedhardcore boolean DEFAULT false NOT NULL
);


ALTER TABLE public.collection_editions OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 25220)
-- Name: collection_editions_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.collection_editions_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.collection_editions_seq OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 25221)
-- Name: craft_data; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.craft_data (
    id bigint NOT NULL,
    dbgameaccount_id bigint,
    ishardcore boolean,
    isseasoned boolean,
    artisan character varying(255),
    level integer,
    learnedrecipes bytea NOT NULL
);


ALTER TABLE public.craft_data OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 25226)
-- Name: craft_data_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.craft_data_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.craft_data_seq OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 25227)
-- Name: game_accounts; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.game_accounts (
    id bigint NOT NULL,
    dbaccount_id bigint,
    lastonline bigint,
    flags integer,
    banner bytea,
    uiprefs bytea,
    uisettings bytea,
    seentutorials bytea,
    bossprogress bytea,
    stashicons bytea,
    paragonlevel integer,
    paragonlevelhardcore integer,
    experience integer,
    experiencehardcore integer,
    lastplayedhero_id bigint,
    gold bigint,
    hardcoregold bigint,
    platinum integer,
    hardplatinum integer,
    rmtcurrency bigint,
    hardrmtcurrency bigint,
    bloodshards integer,
    hardcorebloodshards integer,
    stashsize integer,
    hardcorestashsize integer,
    seasonstashsize integer,
    hardseasonstashsize integer,
    eliteskilled bigint,
    totalkilled bigint,
    totalgold bigint,
    hardtotalgold bigint,
    totalbloodshards integer,
    hardtotalbloodshards integer,
    totalbounties integer DEFAULT 0 NOT NULL,
    totalbountieshardcore integer DEFAULT 0 NOT NULL,
    pvptotalkilled bigint,
    hardpvptotalkilled bigint,
    pvptotalwins bigint,
    hardpvptotalwins bigint,
    pvptotalgold bigint,
    hardpvptotalgold bigint,
    craftitem1 integer,
    hardcraftitem1 integer,
    craftitem2 integer,
    hardcraftitem2 integer,
    craftitem3 integer,
    hardcraftitem3 integer,
    craftitem4 integer,
    hardcraftitem4 integer,
    craftitem5 integer,
    hardcraftitem5 integer,
    vialofputridness integer,
    hardvialofputridness integer,
    idolofterror integer,
    hardidolofterror integer,
    heartoffright integer,
    hardheartoffright integer,
    leorikkey integer,
    hardleorikkey integer,
    bigportalkey integer,
    hardbigportalkey integer,
    horadrica1 integer,
    hardhoradrica1 integer,
    horadrica2 integer,
    hardhoradrica2 integer,
    horadrica3 integer,
    hardhoradrica3 integer,
    horadrica4 integer,
    hardhoradrica4 integer,
    horadrica5 integer,
    hardhoradrica5 integer
);


ALTER TABLE public.game_accounts OWNER TO postgres;

--
-- TOC entry 224 (class 1259 OID 25234)
-- Name: game_accounts_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.game_accounts_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.game_accounts_seq OWNER TO postgres;

--
-- TOC entry 225 (class 1259 OID 25235)
-- Name: global_params; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.global_params (
    id bigint NOT NULL,
    name character varying(255),
    value bigint
);


ALTER TABLE public.global_params OWNER TO postgres;

--
-- TOC entry 226 (class 1259 OID 25238)
-- Name: global_params_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.global_params_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.global_params_seq OWNER TO postgres;

--
-- TOC entry 227 (class 1259 OID 25239)
-- Name: guild_members; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.guild_members (
    id bigint NOT NULL,
    dbguild_id bigint,
    dbgameaccount_id bigint,
    note character varying(50),
    rank integer
);


ALTER TABLE public.guild_members OWNER TO postgres;

--
-- TOC entry 228 (class 1259 OID 25242)
-- Name: guild_news; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.guild_news (
    id bigint NOT NULL,
    dbguild_id bigint,
    dbgameaccount_id bigint,
    type integer,
    "time" bigint,
    data bytea
);


ALTER TABLE public.guild_news OWNER TO postgres;

--
-- TOC entry 229 (class 1259 OID 25247)
-- Name: guildmembers_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.guildmembers_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.guildmembers_seq OWNER TO postgres;

--
-- TOC entry 230 (class 1259 OID 25248)
-- Name: guildnews_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.guildnews_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.guildnews_seq OWNER TO postgres;

--
-- TOC entry 231 (class 1259 OID 25249)
-- Name: guilds; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.guilds (
    id bigint NOT NULL,
    name character varying(50),
    tag character varying(6),
    description character varying(255),
    motd character varying(255),
    category integer,
    language integer,
    islfm boolean,
    isinviterequired boolean,
    rating integer,
    creator_id bigint,
    ranks bytea,
    disbanded boolean
);


ALTER TABLE public.guilds OWNER TO postgres;

--
-- TOC entry 232 (class 1259 OID 25254)
-- Name: guilds_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.guilds_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.guilds_seq OWNER TO postgres;

--
-- TOC entry 233 (class 1259 OID 25255)
-- Name: hireling_data; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.hireling_data (
    id bigint NOT NULL,
    dbtoon_id bigint,
    class integer,
    skill1snoid integer DEFAULT '-1'::integer NOT NULL,
    skill2snoid integer DEFAULT '-1'::integer NOT NULL,
    skill3snoid integer DEFAULT '-1'::integer NOT NULL,
    skill4snoid integer DEFAULT '-1'::integer NOT NULL
);


ALTER TABLE public.hireling_data OWNER TO postgres;

--
-- TOC entry 234 (class 1259 OID 25262)
-- Name: hireling_data_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.hireling_data_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.hireling_data_seq OWNER TO postgres;

--
-- TOC entry 235 (class 1259 OID 25263)
-- Name: items; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.items (
    id bigint NOT NULL,
    dbgameaccount_id bigint,
    dbtoon_id bigint,
    equipmentslot integer,
    forsale boolean DEFAULT false NOT NULL,
    hirelingid integer DEFAULT 0 NOT NULL,
    locationx integer,
    locationy integer,
    ishardcore boolean DEFAULT false NOT NULL,
    unidentified boolean DEFAULT false NOT NULL,
    firstgem integer DEFAULT '-1'::integer NOT NULL,
    secondgem integer DEFAULT '-1'::integer NOT NULL,
    thirdgem integer DEFAULT '-1'::integer NOT NULL,
    gbid integer,
    version integer DEFAULT 1 NOT NULL,
    count integer DEFAULT 1,
    rareitemname bytea,
    dyetype integer DEFAULT 0,
    quality integer DEFAULT 1,
    binding integer DEFAULT 0,
    durability integer DEFAULT 0,
    rating integer DEFAULT 0,
    affixes character varying(255),
    attributes character varying(2500),
    transmoggbid integer DEFAULT '-1'::integer NOT NULL
);


ALTER TABLE public.items OWNER TO postgres;

--
-- TOC entry 236 (class 1259 OID 25283)
-- Name: items_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.items_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.items_seq OWNER TO postgres;

--
-- TOC entry 237 (class 1259 OID 25284)
-- Name: mail; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.mail (
    id bigint NOT NULL,
    dbtoon_id bigint,
    claimed boolean,
    title character varying(255),
    body character varying(255),
    itemgbid integer
);


ALTER TABLE public.mail OWNER TO postgres;

--
-- TOC entry 238 (class 1259 OID 25289)
-- Name: mail_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.mail_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.mail_seq OWNER TO postgres;

--
-- TOC entry 239 (class 1259 OID 25290)
-- Name: quests; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.quests (
    id bigint NOT NULL,
    dbtoon_id bigint,
    questid integer,
    iscompleted boolean DEFAULT false NOT NULL,
    queststep integer
);


ALTER TABLE public.quests OWNER TO postgres;

--
-- TOC entry 240 (class 1259 OID 25294)
-- Name: quests_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.quests_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.quests_seq OWNER TO postgres;

--
-- TOC entry 241 (class 1259 OID 25295)
-- Name: reports; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.reports (
    id bigint NOT NULL,
    type character varying(255),
    dbgameaccount_id bigint,
    dbtoon_id bigint,
    sender_id bigint,
    note character varying(255)
);


ALTER TABLE public.reports OWNER TO postgres;

--
-- TOC entry 242 (class 1259 OID 25300)
-- Name: reports_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.reports_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.reports_seq OWNER TO postgres;

--
-- TOC entry 243 (class 1259 OID 25301)
-- Name: skills; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.skills (
    id bigint NOT NULL,
    dbtoon_id bigint,
    rune0 integer,
    skill0 integer,
    rune1 integer,
    skill1 integer,
    rune2 integer,
    skill2 integer,
    rune3 integer,
    skill3 integer,
    rune4 integer,
    skill4 integer,
    rune5 integer,
    skill5 integer,
    passive0 integer,
    passive1 integer,
    passive2 integer,
    passive3 integer DEFAULT '-1'::integer NOT NULL,
    potiongbid integer DEFAULT '-1'::integer NOT NULL
);


ALTER TABLE public.skills OWNER TO postgres;

--
-- TOC entry 244 (class 1259 OID 25306)
-- Name: skills_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.skills_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.skills_seq OWNER TO postgres;

--
-- TOC entry 245 (class 1259 OID 25307)
-- Name: toons; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.toons (
    id bigint NOT NULL,
    class character varying(255),
    dbgameaccount_id bigint,
    deleted boolean,
    ishardcore boolean,
    isseasoned boolean,
    dead boolean,
    timedeadharcode integer,
    stoneofportal boolean,
    createdseason integer,
    experience integer,
    paragonbonuses bytea,
    pverating integer DEFAULT 0 NOT NULL,
    chestsopened integer DEFAULT 0 NOT NULL,
    eventscompleted integer DEFAULT 0 NOT NULL,
    kills integer DEFAULT 0 NOT NULL,
    deaths integer DEFAULT 0 NOT NULL,
    eliteskilled integer DEFAULT 0 NOT NULL,
    goldgained integer DEFAULT 0 NOT NULL,
    activehireling integer,
    currentact integer,
    currentquestid integer,
    currentqueststepid integer,
    currentdifficulty integer,
    flags character varying(255),
    level smallint,
    stats character varying(255) DEFAULT '0;0;0;0;0;0'::character varying NOT NULL,
    name character varying(255),
    timeplayed integer,
    lore bytea,
    archieved boolean DEFAULT false NOT NULL,
    wingsactive integer DEFAULT '-1'::integer NOT NULL,
    cosmetic1 integer,
    cosmetic2 integer,
    cosmetic3 integer,
    cosmetic4 integer
);


ALTER TABLE public.toons OWNER TO postgres;

--
-- TOC entry 246 (class 1259 OID 25322)
-- Name: toons_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.toons_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.toons_seq OWNER TO postgres;

--
-- TOC entry 3257 (class 2604 OID 25323)
-- Name: DRLG_Container id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."DRLG_Container" ALTER COLUMN id SET DEFAULT nextval('public."DRLG_Container_id_seq"'::regclass);


--
-- TOC entry 3258 (class 2604 OID 25324)
-- Name: DRLG_Tile id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."DRLG_Tile" ALTER COLUMN id SET DEFAULT nextval('public."DRLG_Tile_id_seq"'::regclass);


--
-- TOC entry 3503 (class 0 OID 25184)
-- Dependencies: 209
-- Data for Name: DRLG_Container; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."DRLG_Container" VALUES (6, 452991, 240, NULL);
INSERT INTO public."DRLG_Container" VALUES (7, 452996, 240, NULL);
INSERT INTO public."DRLG_Container" VALUES (11, 288823, 200, NULL);
INSERT INTO public."DRLG_Container" VALUES (16, 331263, 240, NULL);
INSERT INTO public."DRLG_Container" VALUES (17, 360797, 240, NULL);
INSERT INTO public."DRLG_Container" VALUES (18, 452721, 240, NULL);
INSERT INTO public."DRLG_Container" VALUES (19, 452922, 240, NULL);
INSERT INTO public."DRLG_Container" VALUES (20, 452984, 240, NULL);
INSERT INTO public."DRLG_Container" VALUES (21, 452985, 240, NULL);
INSERT INTO public."DRLG_Container" VALUES (22, 452997, 240, NULL);
INSERT INTO public."DRLG_Container" VALUES (23, 452998, 240, NULL);
INSERT INTO public."DRLG_Container" VALUES (24, 288454, 240, NULL);
INSERT INTO public."DRLG_Container" VALUES (1, 0, 240, 50579);
INSERT INTO public."DRLG_Container" VALUES (2, 0, 240, 263494);
INSERT INTO public."DRLG_Container" VALUES (4, 0, 240, 82370);
INSERT INTO public."DRLG_Container" VALUES (5, 0, 240, 82371);
INSERT INTO public."DRLG_Container" VALUES (8, 0, 240, 72636);
INSERT INTO public."DRLG_Container" VALUES (9, 0, 240, 72637);
INSERT INTO public."DRLG_Container" VALUES (10, 0, 240, 154587);
INSERT INTO public."DRLG_Container" VALUES (12, 0, 240, 211471);
INSERT INTO public."DRLG_Container" VALUES (13, 0, 240, 161961);
INSERT INTO public."DRLG_Container" VALUES (14, 0, 240, 50582);
INSERT INTO public."DRLG_Container" VALUES (3, 0, 240, 230288);
INSERT INTO public."DRLG_Container" VALUES (15, 275921, 240, NULL);
INSERT INTO public."DRLG_Container" VALUES (25, 288843, 240, NULL);
INSERT INTO public."DRLG_Container" VALUES (26, 331389, 240, NULL);
INSERT INTO public."DRLG_Container" VALUES (27, 275960, 240, NULL);
INSERT INTO public."DRLG_Container" VALUES (28, 275946, 240, NULL);
INSERT INTO public."DRLG_Container" VALUES (29, 275926, 240, NULL);


--
-- TOC entry 3505 (class 0 OID 25188)
-- Dependencies: 211
-- Data for Name: DRLG_Tile; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."DRLG_Tile" VALUES (1, 1, 0, 32990, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (2, 1, 1, 174633, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (3, 1, 1, 174643, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (4, 1, 1, 174657, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (5, 1, 1, 174663, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (6, 1, 2, 32939, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (7, 1, 2, 32941, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (8, 1, 2, 32951, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (9, 1, 2, 32952, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (10, 1, 2, 32954, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (11, 1, 2, 32955, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (12, 1, 2, 32958, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (13, 1, 2, 32960, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (14, 1, 2, 32961, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (15, 1, 2, 32963, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (16, 1, 2, 32979, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (17, 1, 2, 32981, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (18, 1, 2, 32982, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (19, 1, 2, 32983, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (20, 1, 2, 32985, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (21, 1, 2, 66589, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (22, 1, 2, 66919, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (23, 1, 2, 66925, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (24, 1, 2, 67021, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (25, 1, 2, 32986, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (26, 1, 2, 32987, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (27, 1, 3, 32938, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (28, 1, 3, 32969, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (29, 1, 3, 32970, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (30, 1, 3, 32989, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (31, 1, 3, 1886, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (32, 1, 3, 32943, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (33, 1, 3, 32971, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (34, 1, 3, 32991, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (35, 1, 3, 32999, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (41, 2, 0, 250881, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (42, 2, 0, 250882, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (43, 2, 0, 250883, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (44, 2, 0, 250884, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (45, 2, 1, 250881, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (46, 2, 1, 250882, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (47, 2, 1, 250883, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (48, 2, 1, 250884, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (49, 2, 2, 252804, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (50, 2, 2, 253156, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (51, 2, 2, 289062, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (52, 2, 2, 289135, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (53, 2, 2, 289146, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (54, 2, 2, 253322, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (55, 2, 2, 253395, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (56, 2, 2, 242155, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (57, 2, 2, 242190, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (58, 2, 2, 242272, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (59, 2, 2, 242295, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (60, 2, 2, 242312, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (61, 2, 2, 242326, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (62, 2, 2, 242847, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (63, 2, 2, 242678, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (64, 2, 2, 242711, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (65, 2, 2, 242801, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (66, 2, 2, 242881, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (67, 2, 2, 242935, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (68, 2, 2, 243101, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (69, 2, 2, 243923, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (70, 2, 2, 243990, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (71, 2, 2, 244026, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (72, 2, 2, 244152, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (73, 2, 2, 244271, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (74, 2, 2, 244575, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (75, 2, 2, 244617, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (76, 2, 2, 244655, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (77, 2, 2, 244673, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (78, 2, 2, 244706, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (79, 2, 2, 244722, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (80, 2, 2, 244743, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (81, 2, 2, 244800, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (82, 2, 2, 245024, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (83, 2, 2, 245078, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (84, 2, 2, 245132, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (85, 2, 2, 245192, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (86, 2, 2, 245204, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (87, 2, 2, 245365, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (88, 2, 2, 245507, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (89, 2, 2, 245644, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (90, 2, 3, 244921, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (91, 2, 3, 243005, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (92, 2, 3, 243895, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (93, 2, 3, 243905, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (94, 2, 3, 243954, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (95, 2, 3, 243970, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (96, 2, 3, 244132, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (97, 2, 3, 244230, 263493, -1, 315939);
INSERT INTO public."DRLG_Tile" VALUES (98, 4, 0, 478085, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (99, 4, 0, 77876, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (100, 4, 1, 478079, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (101, 4, 1, 33034, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (102, 4, 1, 33016, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (103, 4, 1, 33041, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (104, 4, 2, 33011, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (105, 4, 2, 33024, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (106, 4, 2, 33025, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (107, 4, 2, 33026, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (108, 4, 2, 33027, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (109, 4, 2, 33029, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (110, 4, 2, 33030, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (111, 4, 2, 33031, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (112, 4, 2, 33037, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (113, 4, 2, 33038, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (114, 4, 3, 33044, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (115, 4, 3, 33039, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (116, 4, 3, 33032, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (117, 4, 3, 33014, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (118, 5, 0, 33040, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (119, 5, 0, 33033, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (120, 5, 1, 33039, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (121, 5, 1, 33044, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (122, 5, 1, 33046, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (123, 5, 1, 33032, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (124, 5, 2, 33011, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (125, 5, 2, 33024, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (126, 5, 2, 33025, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (127, 5, 2, 33026, 82372, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (128, 5, 2, 33027, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (129, 5, 2, 33029, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (130, 5, 2, 33030, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (131, 5, 2, 33031, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (132, 5, 2, 33037, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (133, 5, 2, 33038, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (136, 5, 2, 478081, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (137, 5, 2, 478083, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (138, 5, 3, 478082, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (139, 5, 3, 478084, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (140, 5, 3, 33032, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (141, 5, 3, 33039, 82373, -1, 86469);
INSERT INTO public."DRLG_Tile" VALUES (142, 6, 0, 456189, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (143, 6, 0, 456192, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (144, 6, 0, 456195, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (145, 6, 0, 456197, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (146, 6, 1, 456202, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (147, 6, 1, 456205, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (148, 6, 1, 456208, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (149, 6, 1, 456211, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (150, 6, 2, 456219, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (151, 6, 2, 456222, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (152, 6, 2, 456228, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (153, 6, 2, 456231, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (154, 6, 2, 456233, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (155, 6, 2, 456236, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (156, 6, 2, 456239, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (157, 6, 2, 456242, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (158, 6, 2, 456245, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (159, 6, 2, 456247, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (160, 6, 2, 456250, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (161, 6, 2, 456252, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (162, 6, 2, 456255, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (163, 6, 2, 456258, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (164, 6, 2, 456263, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (165, 6, 2, 456269, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (166, 6, 2, 456273, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (167, 6, 2, 456276, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (168, 6, 2, 456279, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (169, 6, 2, 456282, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (170, 6, 2, 456285, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (171, 6, 2, 456294, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (172, 6, 2, 456295, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (173, 6, 3, 456216, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (174, 6, 3, 456225, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (175, 6, 3, 456266, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (176, 6, 3, 456288, 452992, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (177, 7, 0, 456189, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (178, 7, 0, 456192, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (179, 7, 0, 456195, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (180, 7, 0, 456197, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (181, 7, 1, 456202, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (182, 7, 1, 456205, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (183, 7, 1, 456208, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (184, 7, 1, 456211, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (185, 7, 2, 456219, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (186, 7, 2, 456222, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (187, 7, 2, 456228, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (188, 7, 2, 456231, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (189, 7, 2, 456233, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (190, 7, 2, 456236, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (191, 7, 2, 456239, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (192, 7, 2, 456242, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (193, 7, 2, 456245, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (194, 7, 2, 456247, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (195, 7, 2, 456250, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (196, 7, 2, 456252, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (197, 7, 2, 456255, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (198, 7, 2, 456258, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (199, 7, 2, 456263, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (200, 7, 2, 456269, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (201, 7, 2, 456273, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (202, 7, 2, 456276, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (203, 7, 2, 456279, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (204, 7, 2, 456282, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (205, 7, 2, 456285, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (206, 7, 2, 456294, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (207, 7, 2, 456295, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (208, 7, 3, 456216, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (209, 7, 3, 456225, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (210, 7, 3, 456266, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (211, 7, 3, 456288, 452993, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (217, 8, 0, 33094, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (218, 8, 0, 33057, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (220, 8, 0, 33099, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (221, 8, 0, 61489, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (222, 8, 0, 61650, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (223, 8, 0, 61567, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (224, 8, 1, 84766, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (225, 8, 1, 84775, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (226, 8, 1, 84784, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (227, 8, 1, 84787, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (228, 8, 2, 33051, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (229, 8, 2, 33054, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (230, 8, 2, 33066, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (231, 8, 2, 33068, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (232, 8, 2, 33070, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (235, 8, 2, 33073, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (236, 8, 2, 33075, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (237, 8, 2, 33076, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (238, 8, 2, 33077, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (239, 8, 2, 33078, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (240, 8, 2, 33086, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (241, 8, 2, 33087, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (242, 8, 2, 33091, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (243, 8, 2, 33092, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (244, 8, 2, 1890, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (245, 8, 3, 61237, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (246, 8, 3, 61403, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (247, 8, 3, 61428, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (248, 8, 3, 61449, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (274, 1, 4, 32936, 19780, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (275, 12, 0, 211486, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (276, 12, 0, 211488, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (277, 12, 0, 211490, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (278, 12, 0, 211492, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (279, 12, 1, 343946, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (280, 12, 1, 343947, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (281, 12, 1, 343948, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (282, 12, 1, 343949, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (283, 12, 2, 209181, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (284, 12, 2, 209225, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (285, 12, 2, 204737, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (286, 12, 2, 204745, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (287, 12, 2, 204755, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (288, 12, 2, 204765, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (289, 12, 2, 204775, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (290, 12, 3, 204624, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (291, 12, 3, 204788, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (292, 12, 3, 204804, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (252, 11, 0, 31100, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (254, 11, 1, 31102, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (256, 11, 1, 31083, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (257, 11, 1, 31096, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (258, 11, 2, 31079, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (259, 11, 2, 31085, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (260, 11, 2, 31086, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (261, 11, 2, 31087, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (262, 11, 2, 31088, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (263, 11, 2, 31089, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (264, 11, 2, 31090, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (265, 11, 2, 31091, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (266, 11, 2, 31092, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (267, 11, 2, 31097, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (269, 11, 3, 31080, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (270, 11, 3, 31093, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (272, 11, 3, 31103, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (293, 12, 3, 204814, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (294, 12, 4, 204614, 211479, -1, 206287);
INSERT INTO public."DRLG_Tile" VALUES (295, 13, 4, 154775, 161964, -1, 2809);
INSERT INTO public."DRLG_Tile" VALUES (296, 13, 3, 132203, 161964, -1, 2809);
INSERT INTO public."DRLG_Tile" VALUES (297, 13, 0, 132218, 161964, -1, 2809);
INSERT INTO public."DRLG_Tile" VALUES (298, 13, 3, 132263, 161964, -1, 2809);
INSERT INTO public."DRLG_Tile" VALUES (299, 13, 3, 132293, 161964, -1, 2809);
INSERT INTO public."DRLG_Tile" VALUES (300, 13, 2, 131902, 161964, -1, 2809);
INSERT INTO public."DRLG_Tile" VALUES (301, 15, 4, 32936, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (302, 15, 0, 32940, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (303, 15, 1, 32948, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (304, 15, 1, 32997, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (305, 15, 1, 33002, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (306, -1, 1, -1, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (307, 15, 3, 32999, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (308, 15, 3, 32971, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (309, 15, 3, 32991, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (310, 15, 3, 32943, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (311, 15, 2, 32981, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (312, 15, 2, 32985, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (313, 15, 2, 32982, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (314, 15, 2, 32983, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (315, 15, 2, 32965, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (316, 15, 2, 32967, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (317, 15, 2, 32951, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (318, 15, 2, 32952, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (319, 15, 2, 32939, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (320, 15, 2, 32941, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (321, 15, 2, 32961, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (322, 15, 2, 32963, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (323, 15, 2, 32954, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (324, 15, 2, 32958, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (325, 15, 2, 32979, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (326, 15, 2, 32960, 19785, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (327, 14, 4, 32936, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (328, 14, 0, 32944, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (329, 14, 0, 32972, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (330, 14, 0, 32992, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (331, 14, 0, 33000, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (332, 14, 1, 32976, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (333, 14, 1, 33001, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (334, 14, 1, 32946, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (335, 14, 3, 32999, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (336, 14, 3, 32971, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (337, 14, 3, 32991, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (338, 14, 3, 32943, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (339, 14, 2, 32981, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (340, 14, 2, 32985, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (341, 14, 2, 32982, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (342, 14, 2, 32983, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (343, 14, 2, 32965, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (344, 14, 2, 32967, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (345, 14, 2, 32951, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (346, 14, 2, 32952, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (347, 14, 2, 32939, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (348, 14, 2, 32941, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (349, 14, 2, 32961, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (350, 14, 2, 32963, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (351, 14, 2, 32954, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (352, 14, 2, 32958, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (353, 14, 2, 32979, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (354, 14, 2, 32960, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (355, 14, 2, 32984, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (356, 14, 2, 66589, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (357, 14, 2, 66919, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (358, 14, 2, 66925, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (359, 14, 2, 67021, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (360, 14, 2, 1883, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (361, 14, 2, 1884, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (362, 14, 2, 1885, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (363, 14, 1, 32993, 19783, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (445, 18, 4, 32936, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (446, 18, 0, 32944, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (447, 18, 0, 32972, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (448, 18, 0, 32992, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (449, 18, 0, 33000, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (450, 18, 1, 32976, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (451, 18, 1, 33001, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (452, 18, 1, 32946, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (414, 17, 0, 270859, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (415, 17, 0, 270960, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (417, 17, 1, 271019, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (418, 17, 1, 270973, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (419, 17, 1, 270873, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (420, 17, 1, 270931, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (421, 17, 3, 269281, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (422, 17, 3, 269240, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (423, 17, 3, 269253, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (424, 17, 3, 269271, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (425, 17, 2, 269081, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (411, 16, 2, 252804, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (365, 16, 0, 337235, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (366, 16, 0, 337236, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (368, 16, 0, 337238, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (369, 16, 1, 250892, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (371, 16, 1, 250894, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (372, 16, 1, 250895, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (373, 16, 3, 243005, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (374, 16, 3, 243895, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (410, 16, 2, 244271, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (409, 16, 2, 244026, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (408, 16, 2, 245204, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (407, 16, 2, 244800, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (406, 16, 2, 242155, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (405, 16, 2, 289062, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (404, 16, 2, 245132, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (403, 16, 2, 245024, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (402, 16, 2, 245078, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (401, 16, 2, 242295, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (398, 16, 2, 244152, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (397, 16, 2, 243101, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (396, 16, 2, 242801, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (375, 16, 3, 243905, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (376, 16, 3, 243954, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (377, 16, 3, 244132, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (378, 16, 3, 244230, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (379, 16, 2, 242678, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (380, 16, 2, 244673, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (381, 16, 2, 244743, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (383, 16, 2, 243990, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (384, 16, 2, 253156, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (385, 16, 2, 253322, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (386, 16, 2, 253395, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (388, 16, 2, 242312, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (389, 16, 2, 245192, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (390, 16, 2, 244617, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (391, 16, 2, 242272, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (392, 16, 2, 245365, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (393, 16, 2, 245507, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (394, 16, 2, 245644, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (395, 16, 2, 242190, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (453, 18, 3, 32999, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (454, 18, 3, 32971, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (455, 18, 3, 32991, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (456, 18, 3, 32943, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (457, 18, 2, 32981, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (458, 18, 2, 32985, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (459, 18, 2, 32982, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (460, 18, 2, 32983, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (461, 18, 2, 32965, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (462, 18, 2, 32967, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (463, 18, 2, 32951, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (464, 18, 2, 32952, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (465, 18, 2, 32939, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (466, 18, 2, 32941, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (467, 18, 2, 32961, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (468, 18, 2, 32963, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (469, 18, 2, 32954, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (470, 18, 2, 32958, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (471, 18, 2, 32979, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (472, 18, 2, 32960, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (473, 18, 2, 32984, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (474, 18, 2, 66589, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (475, 18, 2, 66919, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (476, 18, 2, 66925, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (477, 18, 2, 67021, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (478, 18, 2, 1883, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (479, 18, 2, 1884, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (480, 18, 2, 1885, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (481, 18, 1, 32993, 452986, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (482, 19, 4, 32936, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (483, 19, 0, 32944, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (484, 19, 0, 32972, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (485, 19, 0, 32992, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (486, 19, 0, 33000, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (487, 19, 1, 32976, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (488, 19, 1, 33001, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (489, 19, 1, 32946, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (490, 19, 3, 32999, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (491, 19, 3, 32971, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (492, 19, 3, 32991, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (493, 19, 3, 32943, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (494, 19, 2, 32981, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (495, 19, 2, 32985, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (496, 19, 2, 32982, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (497, 19, 2, 32983, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (498, 19, 2, 32965, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (499, 19, 2, 32967, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (500, 19, 2, 32951, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (501, 19, 2, 32952, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (502, 19, 2, 32939, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (503, 19, 2, 32941, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (504, 19, 2, 32961, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (505, 19, 2, 32963, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (506, 19, 2, 32954, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (507, 19, 2, 32958, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (508, 19, 2, 32979, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (509, 19, 2, 32960, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (510, 19, 2, 32984, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (511, 19, 2, 66589, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (512, 19, 2, 66919, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (513, 19, 2, 66925, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (514, 19, 2, 67021, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (515, 19, 2, 1883, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (516, 19, 2, 1884, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (517, 19, 2, 1885, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (518, 19, 1, 32993, 452988, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (519, 20, 0, 32944, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (520, 20, 0, 32972, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (521, 20, 0, 32992, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (522, 20, 0, 33000, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (523, 20, 1, 32976, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (524, 20, 1, 33001, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (525, 20, 1, 32946, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (526, 20, 3, 32999, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (527, 20, 3, 32971, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (528, 20, 3, 32991, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (529, 20, 3, 32943, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (530, 20, 2, 32981, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (531, 20, 2, 32985, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (532, 20, 2, 32982, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (533, 20, 2, 32983, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (534, 20, 2, 32965, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (535, 20, 2, 32967, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (536, 20, 2, 32951, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (537, 20, 2, 32952, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (538, 20, 2, 32939, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (539, 20, 2, 32941, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (540, 20, 2, 32961, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (541, 20, 2, 32963, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (542, 20, 2, 32954, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (543, 20, 2, 32958, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (544, 20, 2, 32979, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (545, 20, 2, 32960, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (546, 20, 2, 32984, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (547, 20, 2, 66589, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (548, 20, 2, 66919, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (549, 20, 2, 66925, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (550, 20, 2, 67021, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (551, 20, 2, 1883, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (552, 20, 2, 1884, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (553, 20, 2, 1885, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (554, 20, 1, 32993, 452989, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (555, 21, 0, 32944, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (556, 21, 0, 32972, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (557, 21, 0, 32992, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (558, 21, 0, 33000, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (559, 21, 1, 32976, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (560, 21, 1, 33001, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (561, 21, 1, 32946, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (562, 21, 3, 32999, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (563, 21, 3, 32971, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (564, 21, 3, 32991, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (565, 21, 3, 32943, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (566, 21, 2, 32981, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (567, 21, 2, 32985, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (568, 21, 2, 32982, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (569, 21, 2, 32983, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (570, 21, 2, 32965, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (571, 21, 2, 32967, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (572, 21, 2, 32951, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (428, 17, 2, 269427, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (429, 17, 2, 268554, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (442, 17, 2, 271661, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (440, 17, 2, 271842, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (439, 17, 2, 268682, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (438, 17, 2, 271696, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (437, 17, 2, 269047, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (436, 17, 2, 271571, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (435, 17, 2, 268943, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (434, 17, 2, 271760, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (433, 17, 2, 269204, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (430, 17, 2, 272180, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (431, 17, 2, 268617, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (432, 17, 2, 271995, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (573, 21, 2, 32952, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (574, 21, 2, 32939, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (575, 21, 2, 32941, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (576, 21, 2, 32961, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (577, 21, 2, 32963, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (578, 21, 2, 32954, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (579, 21, 2, 32958, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (580, 21, 2, 32979, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (581, 21, 2, 32960, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (582, 21, 2, 32984, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (583, 21, 2, 66589, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (584, 21, 2, 66919, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (585, 21, 2, 66925, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (586, 21, 2, 67021, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (587, 21, 2, 1883, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (588, 21, 2, 1884, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (589, 21, 2, 1885, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (590, 21, 1, 32993, 452990, -1, 50535);
INSERT INTO public."DRLG_Tile" VALUES (591, 22, 0, 456189, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (592, 22, 0, 456192, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (593, 22, 0, 456195, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (594, 22, 0, 456197, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (595, 22, 1, 456202, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (596, 22, 1, 456205, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (597, 22, 1, 456208, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (598, 22, 1, 456211, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (599, 22, 2, 456219, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (600, 22, 2, 456222, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (601, 22, 2, 456228, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (602, 22, 2, 456231, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (603, 22, 2, 456233, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (604, 22, 2, 456236, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (605, 22, 2, 456239, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (606, 22, 2, 456242, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (607, 22, 2, 456245, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (608, 22, 2, 456247, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (609, 22, 2, 456250, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (610, 22, 2, 456252, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (611, 22, 2, 456255, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (612, 22, 2, 456258, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (613, 22, 2, 456263, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (614, 22, 2, 456269, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (615, 22, 2, 456273, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (616, 22, 2, 456276, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (617, 22, 2, 456279, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (618, 22, 2, 456282, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (619, 22, 2, 456285, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (620, 22, 2, 456294, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (621, 22, 2, 456295, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (622, 22, 3, 456216, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (623, 22, 3, 456225, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (624, 22, 3, 456266, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (625, 22, 3, 456288, 452994, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (626, 23, 0, 456189, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (627, 23, 0, 456192, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (628, 23, 0, 456195, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (629, 23, 0, 456197, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (630, 23, 1, 456202, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (631, 23, 1, 456205, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (632, 23, 1, 456208, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (633, 23, 1, 456211, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (634, 23, 2, 456219, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (635, 23, 2, 456222, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (636, 23, 2, 456228, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (637, 23, 2, 456231, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (638, 23, 2, 456233, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (639, 23, 2, 456236, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (640, 23, 2, 456239, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (641, 23, 2, 456242, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (642, 23, 2, 456245, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (643, 23, 2, 456247, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (644, 23, 2, 456250, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (645, 23, 2, 456252, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (646, 23, 2, 456255, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (647, 23, 2, 456258, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (648, 23, 2, 456263, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (649, 23, 2, 456269, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (650, 23, 2, 456273, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (651, 23, 2, 456276, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (652, 23, 2, 456279, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (653, 23, 2, 456282, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (654, 23, 2, 456285, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (655, 23, 2, 456294, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (656, 23, 2, 456295, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (657, 23, 3, 456216, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (658, 23, 3, 456225, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (659, 23, 3, 456266, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (660, 23, 3, 456288, 452995, -1, 453221);
INSERT INTO public."DRLG_Tile" VALUES (661, 10, 4, 33060, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (662, 10, 0, 33094, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (663, 10, 0, 33099, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (664, -1, 0, 33057, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (665, -1, 0, 61489, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (666, -1, 0, 61567, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (667, -1, 0, 61650, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (669, 10, 1, 84766, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (670, 10, 1, 84775, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (671, 10, 1, 84784, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (672, 10, 1, 84787, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (673, 10, 2, 1890, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (674, 10, 2, 33066, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (675, 10, 2, 33068, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (676, 10, 2, 33070, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (677, 10, 2, 33071, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (678, 10, 2, 33072, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (679, 10, 2, 33073, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (680, 10, 2, 33075, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (681, 10, 2, 33076, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (682, 10, 2, 33077, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (683, 10, 2, 33078, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (684, 10, 2, 33086, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (685, 10, 2, 33087, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (686, 10, 2, 33091, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (687, 10, 2, 33092, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (688, 10, 3, 61237, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (689, 10, 3, 61403, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (690, 10, 3, 61428, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (691, 10, 3, 61449, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (692, 24, 0, 151979, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (693, 24, 0, 154056, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (694, 24, 0, 154731, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (695, 24, 0, 154821, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (696, 24, 1, 155325, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (697, 24, 1, 154066, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (698, 24, 1, 156637, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (699, 24, 1, 156649, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (700, 24, 2, 221307, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (701, 24, 2, 151143, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (702, 24, 2, 151319, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (703, 24, 2, 151698, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (704, 24, 2, 133974, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (705, 24, 2, 152038, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (706, 24, 2, 152562, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (707, 24, 2, 152610, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (708, 24, 2, 152935, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (709, 24, 2, 153112, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (710, 24, 2, 153250, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (711, 24, 2, 153287, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (712, 24, 2, 154118, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (713, 24, 2, 154189, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (714, 24, 2, 154223, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (715, 24, 2, 154639, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (716, 24, 2, 460991, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (717, 24, 2, 464560, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (718, 24, 3, 151655, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (719, 24, 3, 154038, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (720, 24, 3, 154687, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (721, 24, 3, 154791, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (722, 24, 3, 460986, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (723, 24, 4, 154952, 288482, -1, 214225);
INSERT INTO public."DRLG_Tile" VALUES (724, 9, 0, 33094, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (725, 9, 0, 33057, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (726, 9, 0, 33099, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (727, 9, 0, 61489, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (728, 9, 0, 61650, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (729, 9, 0, 61567, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (730, 9, 1, 84766, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (731, 9, 1, 84775, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (732, 9, 1, 84784, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (733, 9, 1, 84787, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (734, 9, 2, 33051, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (735, 9, 2, 33054, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (736, 9, 2, 33066, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (737, 9, 2, 33068, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (738, 9, 2, 33070, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (739, 9, 2, 33073, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (740, 9, 2, 33075, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (741, 9, 2, 33076, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (742, 9, 2, 33077, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (743, 9, 2, 33078, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (744, 9, 2, 33086, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (745, 9, 2, 33087, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (746, 9, 2, 33091, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (747, 9, 2, 33092, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (748, 9, 2, 1890, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (749, 9, 3, 61237, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (750, 9, 3, 61403, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (751, 9, 3, 61428, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (752, 9, 3, 61449, 154588, -1, 50542);
INSERT INTO public."DRLG_Tile" VALUES (412, 17, 4, 269728, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (413, 17, 0, 270898, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (416, 17, 0, 271005, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (426, 17, 2, 271684, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (427, 17, 2, 269063, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (444, 17, 2, 271724, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (443, 17, 2, 269227, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (441, 17, 2, 269183, 333758, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (364, 16, 4, 313270, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (367, 16, 0, 337237, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (370, 16, 1, 250893, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (400, 16, 2, 242711, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (399, 16, 2, 244575, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (382, 16, 2, 242326, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (762, 26, 2, 292833, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (763, 26, 2, 292969, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (764, 26, 2, 271073, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (765, 26, 2, 293035, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (766, 26, 2, 293233, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (767, 26, 2, 293329, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (768, 26, 2, 294284, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (387, 16, 2, 289135, 331264, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (806, 28, 3, 61237, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (789, 27, 2, 188872, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (790, 27, 2, 188884, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (791, 27, 2, 188896, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (792, 27, 2, 188909, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (793, 27, 2, 188926, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (794, 27, 2, 188940, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (777, 27, 4, 188860, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (769, 26, 3, 288966, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (770, 26, 3, 289231, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (771, 26, 3, 292911, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (761, 26, 4, 288276, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (753, 26, 0, 293459, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (754, 26, 0, 293469, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (755, 26, 0, 293479, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (756, 26, 0, 293500, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (757, 26, 1, 293489, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (758, 26, 1, 293512, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (759, 26, 1, 293522, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (760, 26, 1, 293540, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (797, 27, 2, 189028, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (782, 27, 3, 188842, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (773, 27, 0, 188973, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (774, 27, 0, 189120, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (775, 27, 0, 189152, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (772, 26, 3, 292957, 352719, -1, 461364);
INSERT INTO public."DRLG_Tile" VALUES (250, 11, 0, 31081, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (251, 11, 0, 31094, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (253, 11, 0, 31105, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (255, 11, 1, 31107, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (268, 11, 2, 31098, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (271, 11, 3, 31099, 312607, -1, -1);
INSERT INTO public."DRLG_Tile" VALUES (776, 27, 0, 188852, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (778, 27, 1, 189312, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (779, 27, 1, 189323, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (780, 27, 1, 189324, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (781, 27, 1, 189325, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (783, 27, 3, 188957, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (784, 27, 3, 189089, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (785, 27, 3, 189137, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (786, 27, 2, 188753, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (787, 27, 2, 189322, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (795, 27, 2, 189003, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (796, 27, 2, 189016, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (804, 28, 1, 175020, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (799, 28, 0, 33082, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (805, 28, 1, 175021, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (788, 27, 2, 188826, 276230, -1, 188757);
INSERT INTO public."DRLG_Tile" VALUES (809, 28, 3, 61449, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (800, 28, 0, 33094, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (807, 28, 3, 61403, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (811, 28, 2, 33071, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (813, 28, 2, 33073, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (808, 28, 3, 61428, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (812, 28, 2, 33066, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (814, 28, 2, 33068, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (815, 28, 2, 33070, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (816, 28, 2, 33051, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (818, 28, 2, 33075, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (819, 28, 2, 33076, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (820, 28, 2, 33077, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (798, 28, 0, 33057, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (801, 28, 0, 33099, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (802, 28, 1, 175016, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (846, 29, 2, 153112, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (847, 29, 2, 153250, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (848, 29, 2, 153287, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (849, 29, 2, 154118, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (850, 29, 2, 154189, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (851, 29, 2, 154223, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (852, 29, 2, 154639, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (853, 29, 2, 151698, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (854, 29, 3, 154791, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (855, 29, 3, 154687, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (856, 29, 3, 154038, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (857, 29, 3, 151655, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (858, 29, 4, 154952, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (803, 28, 1, 175018, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (810, 28, 4, 33060, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (817, 28, 2, 33054, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (821, 28, 2, 33078, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (822, 28, 2, 33068, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (823, 28, 2, 33087, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (824, 28, 2, 33091, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (825, 28, 2, 33092, 276150, -1, 339913);
INSERT INTO public."DRLG_Tile" VALUES (859, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."DRLG_Tile" VALUES (826, 29, 0, 151979, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (827, 29, 0, 154056, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (828, 29, 0, 154731, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (829, 29, 0, 154821, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (830, 29, 1, 261453, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (831, 29, 1, 261454, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (832, 29, 1, 261455, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (833, 29, 1, 261456, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (834, 29, 2, 220797, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (835, 29, 2, 221307, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (836, 29, 2, 151143, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (837, 29, 2, 151319, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (860, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."DRLG_Tile" VALUES (861, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO public."DRLG_Tile" VALUES (838, 29, 2, 133974, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (839, 29, 2, 152038, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (840, 29, 2, 223742, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (841, 29, 2, 223766, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (842, 29, 2, 152562, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (843, 29, 2, 152610, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (844, 29, 2, 152935, 275929, -1, 339488);
INSERT INTO public."DRLG_Tile" VALUES (845, 29, 2, 224644, 275929, -1, 339488);


--
-- TOC entry 3507 (class 0 OID 25192)
-- Dependencies: 213
-- Data for Name: account_relations; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3509 (class 0 OID 25197)
-- Dependencies: 215
-- Data for Name: accounts; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3511 (class 0 OID 25207)
-- Dependencies: 217
-- Data for Name: achievements; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3513 (class 0 OID 25215)
-- Dependencies: 219
-- Data for Name: collection_editions; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3515 (class 0 OID 25221)
-- Dependencies: 221
-- Data for Name: craft_data; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3517 (class 0 OID 25227)
-- Dependencies: 223
-- Data for Name: game_accounts; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3519 (class 0 OID 25235)
-- Dependencies: 225
-- Data for Name: global_params; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3521 (class 0 OID 25239)
-- Dependencies: 227
-- Data for Name: guild_members; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3522 (class 0 OID 25242)
-- Dependencies: 228
-- Data for Name: guild_news; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3525 (class 0 OID 25249)
-- Dependencies: 231
-- Data for Name: guilds; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3527 (class 0 OID 25255)
-- Dependencies: 233
-- Data for Name: hireling_data; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3529 (class 0 OID 25263)
-- Dependencies: 235
-- Data for Name: items; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3531 (class 0 OID 25284)
-- Dependencies: 237
-- Data for Name: mail; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3533 (class 0 OID 25290)
-- Dependencies: 239
-- Data for Name: quests; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3535 (class 0 OID 25295)
-- Dependencies: 241
-- Data for Name: reports; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3537 (class 0 OID 25301)
-- Dependencies: 243
-- Data for Name: skills; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3539 (class 0 OID 25307)
-- Dependencies: 245
-- Data for Name: toons; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3550 (class 0 OID 0)
-- Dependencies: 210
-- Name: DRLG_Container_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."DRLG_Container_id_seq"', 1, false);


--
-- TOC entry 3551 (class 0 OID 0)
-- Dependencies: 212
-- Name: DRLG_Tile_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."DRLG_Tile_id_seq"', 1, false);


--
-- TOC entry 3552 (class 0 OID 0)
-- Dependencies: 214
-- Name: account_relations_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.account_relations_seq', 1, false);


--
-- TOC entry 3553 (class 0 OID 0)
-- Dependencies: 216
-- Name: accounts_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.accounts_seq', 1, false);


--
-- TOC entry 3554 (class 0 OID 0)
-- Dependencies: 218
-- Name: achievements_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.achievements_seq', 1, false);


--
-- TOC entry 3555 (class 0 OID 0)
-- Dependencies: 220
-- Name: collection_editions_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.collection_editions_seq', 1, false);


--
-- TOC entry 3556 (class 0 OID 0)
-- Dependencies: 222
-- Name: craft_data_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.craft_data_seq', 1, false);


--
-- TOC entry 3557 (class 0 OID 0)
-- Dependencies: 224
-- Name: game_accounts_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.game_accounts_seq', 1, false);


--
-- TOC entry 3558 (class 0 OID 0)
-- Dependencies: 226
-- Name: global_params_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.global_params_seq', 1, false);


--
-- TOC entry 3559 (class 0 OID 0)
-- Dependencies: 229
-- Name: guildmembers_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.guildmembers_seq', 1, false);


--
-- TOC entry 3560 (class 0 OID 0)
-- Dependencies: 230
-- Name: guildnews_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.guildnews_seq', 1, false);


--
-- TOC entry 3561 (class 0 OID 0)
-- Dependencies: 232
-- Name: guilds_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.guilds_seq', 1, false);


--
-- TOC entry 3562 (class 0 OID 0)
-- Dependencies: 234
-- Name: hireling_data_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.hireling_data_seq', 1, false);


--
-- TOC entry 3563 (class 0 OID 0)
-- Dependencies: 236
-- Name: items_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.items_seq', 1, false);


--
-- TOC entry 3564 (class 0 OID 0)
-- Dependencies: 238
-- Name: mail_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.mail_seq', 1, false);


--
-- TOC entry 3565 (class 0 OID 0)
-- Dependencies: 240
-- Name: quests_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.quests_seq', 1, false);


--
-- TOC entry 3566 (class 0 OID 0)
-- Dependencies: 242
-- Name: reports_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.reports_seq', 1, false);


--
-- TOC entry 3567 (class 0 OID 0)
-- Dependencies: 244
-- Name: skills_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.skills_seq', 1, false);


--
-- TOC entry 3568 (class 0 OID 0)
-- Dependencies: 246
-- Name: toons_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.toons_seq', 1, false);


--
-- TOC entry 3303 (class 2606 OID 25326)
-- Name: DRLG_Container DRLG_Container_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."DRLG_Container"
    ADD CONSTRAINT "DRLG_Container_pkey" PRIMARY KEY (id);


--
-- TOC entry 3305 (class 2606 OID 25328)
-- Name: DRLG_Tile DRLG_Tile_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."DRLG_Tile"
    ADD CONSTRAINT "DRLG_Tile_pkey" PRIMARY KEY (id);


--
-- TOC entry 3307 (class 2606 OID 25330)
-- Name: account_relations account_relations_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.account_relations
    ADD CONSTRAINT account_relations_pkey PRIMARY KEY (id);


--
-- TOC entry 3309 (class 2606 OID 25332)
-- Name: accounts accounts_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accounts
    ADD CONSTRAINT accounts_pkey PRIMARY KEY (id);


--
-- TOC entry 3311 (class 2606 OID 25334)
-- Name: achievements achievements_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.achievements
    ADD CONSTRAINT achievements_pkey PRIMARY KEY (id);


--
-- TOC entry 3313 (class 2606 OID 25336)
-- Name: collection_editions collection_editions_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.collection_editions
    ADD CONSTRAINT collection_editions_pkey PRIMARY KEY (id);


--
-- TOC entry 3315 (class 2606 OID 25338)
-- Name: craft_data craft_data_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.craft_data
    ADD CONSTRAINT craft_data_pkey PRIMARY KEY (id);


--
-- TOC entry 3317 (class 2606 OID 25340)
-- Name: game_accounts game_accounts_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.game_accounts
    ADD CONSTRAINT game_accounts_pkey PRIMARY KEY (id);


--
-- TOC entry 3319 (class 2606 OID 25342)
-- Name: global_params global_params_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.global_params
    ADD CONSTRAINT global_params_pkey PRIMARY KEY (id);


--
-- TOC entry 3321 (class 2606 OID 25344)
-- Name: guild_members guild_members_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.guild_members
    ADD CONSTRAINT guild_members_pkey PRIMARY KEY (id);


--
-- TOC entry 3323 (class 2606 OID 25346)
-- Name: guild_news guild_news_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.guild_news
    ADD CONSTRAINT guild_news_pkey PRIMARY KEY (id);


--
-- TOC entry 3325 (class 2606 OID 25348)
-- Name: guilds guilds_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.guilds
    ADD CONSTRAINT guilds_pkey PRIMARY KEY (id);


--
-- TOC entry 3327 (class 2606 OID 25350)
-- Name: hireling_data hireling_data_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.hireling_data
    ADD CONSTRAINT hireling_data_pkey PRIMARY KEY (id);


--
-- TOC entry 3329 (class 2606 OID 25352)
-- Name: items items_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.items
    ADD CONSTRAINT items_pkey PRIMARY KEY (id);


--
-- TOC entry 3331 (class 2606 OID 25354)
-- Name: mail mail_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.mail
    ADD CONSTRAINT mail_pkey PRIMARY KEY (id);


--
-- TOC entry 3333 (class 2606 OID 25356)
-- Name: quests quests_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.quests
    ADD CONSTRAINT quests_pkey PRIMARY KEY (id);


--
-- TOC entry 3335 (class 2606 OID 25358)
-- Name: reports reports_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.reports
    ADD CONSTRAINT reports_pkey PRIMARY KEY (id);


--
-- TOC entry 3337 (class 2606 OID 25360)
-- Name: skills skills_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.skills
    ADD CONSTRAINT skills_pkey PRIMARY KEY (id);


--
-- TOC entry 3339 (class 2606 OID 25362)
-- Name: toons toons_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.toons
    ADD CONSTRAINT toons_pkey PRIMARY KEY (id);


--
-- TOC entry 3362 (class 2606 OID 25363)
-- Name: skills fk_13ada7d3; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.skills
    ADD CONSTRAINT fk_13ada7d3 FOREIGN KEY (dbtoon_id) REFERENCES public.toons(id);


--
-- TOC entry 3343 (class 2606 OID 25368)
-- Name: achievements fk_18070981; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.achievements
    ADD CONSTRAINT fk_18070981 FOREIGN KEY (dbgameaccount_id) REFERENCES public.game_accounts(id);


--
-- TOC entry 3359 (class 2606 OID 25373)
-- Name: reports fk_1e0edf25; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.reports
    ADD CONSTRAINT fk_1e0edf25 FOREIGN KEY (sender_id) REFERENCES public.game_accounts(id);


--
-- TOC entry 3360 (class 2606 OID 25378)
-- Name: reports fk_2e7cba8c; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.reports
    ADD CONSTRAINT fk_2e7cba8c FOREIGN KEY (dbgameaccount_id) REFERENCES public.game_accounts(id);


--
-- TOC entry 3355 (class 2606 OID 25383)
-- Name: items fk_30a35cbe; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.items
    ADD CONSTRAINT fk_30a35cbe FOREIGN KEY (dbtoon_id) REFERENCES public.toons(id);


--
-- TOC entry 3346 (class 2606 OID 25388)
-- Name: craft_data fk_3b963ff3; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.craft_data
    ADD CONSTRAINT fk_3b963ff3 FOREIGN KEY (dbgameaccount_id) REFERENCES public.game_accounts(id);


--
-- TOC entry 3351 (class 2606 OID 25393)
-- Name: guild_news fk_514446fe; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.guild_news
    ADD CONSTRAINT fk_514446fe FOREIGN KEY (dbguild_id) REFERENCES public.guilds(id);


--
-- TOC entry 3354 (class 2606 OID 25398)
-- Name: hireling_data fk_5fc5407c; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.hireling_data
    ADD CONSTRAINT fk_5fc5407c FOREIGN KEY (dbtoon_id) REFERENCES public.toons(id);


--
-- TOC entry 3361 (class 2606 OID 25403)
-- Name: reports fk_6a424906; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.reports
    ADD CONSTRAINT fk_6a424906 FOREIGN KEY (dbtoon_id) REFERENCES public.toons(id);


--
-- TOC entry 3347 (class 2606 OID 25408)
-- Name: game_accounts fk_6a4a92e7; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.game_accounts
    ADD CONSTRAINT fk_6a4a92e7 FOREIGN KEY (lastplayedhero_id) REFERENCES public.toons(id);


--
-- TOC entry 3357 (class 2606 OID 25413)
-- Name: mail fk_731b838f; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.mail
    ADD CONSTRAINT fk_731b838f FOREIGN KEY (dbtoon_id) REFERENCES public.toons(id);


--
-- TOC entry 3358 (class 2606 OID 25418)
-- Name: quests fk_87f0b0fa; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.quests
    ADD CONSTRAINT fk_87f0b0fa FOREIGN KEY (dbtoon_id) REFERENCES public.toons(id);


--
-- TOC entry 3349 (class 2606 OID 25423)
-- Name: guild_members fk_8da26c16; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.guild_members
    ADD CONSTRAINT fk_8da26c16 FOREIGN KEY (dbgameaccount_id) REFERENCES public.game_accounts(id);


--
-- TOC entry 3340 (class 2606 OID 25428)
-- Name: account_relations fk_90774aff; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.account_relations
    ADD CONSTRAINT fk_90774aff FOREIGN KEY (listtarget_id) REFERENCES public.accounts(id);


--
-- TOC entry 3344 (class 2606 OID 25433)
-- Name: collection_editions fk_99f1c08; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.collection_editions
    ADD CONSTRAINT fk_99f1c08 FOREIGN KEY (dbaccount_id) REFERENCES public.accounts(id);


--
-- TOC entry 3353 (class 2606 OID 25438)
-- Name: guilds fk_9f1b0c33; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.guilds
    ADD CONSTRAINT fk_9f1b0c33 FOREIGN KEY (creator_id) REFERENCES public.game_accounts(id);


--
-- TOC entry 3352 (class 2606 OID 25443)
-- Name: guild_news fk_ab47ab3b; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.guild_news
    ADD CONSTRAINT fk_ab47ab3b FOREIGN KEY (dbgameaccount_id) REFERENCES public.game_accounts(id);


--
-- TOC entry 3356 (class 2606 OID 25448)
-- Name: items fk_b1c0d926; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.items
    ADD CONSTRAINT fk_b1c0d926 FOREIGN KEY (dbgameaccount_id) REFERENCES public.game_accounts(id);


--
-- TOC entry 3345 (class 2606 OID 25453)
-- Name: collection_editions fk_bd7cdba7; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.collection_editions
    ADD CONSTRAINT fk_bd7cdba7 FOREIGN KEY (claimedtoon_id) REFERENCES public.toons(id);


--
-- TOC entry 3341 (class 2606 OID 25458)
-- Name: account_relations fk_c49249a1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.account_relations
    ADD CONSTRAINT fk_c49249a1 FOREIGN KEY (listowner_id) REFERENCES public.accounts(id);


--
-- TOC entry 3350 (class 2606 OID 25463)
-- Name: guild_members fk_d675c298; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.guild_members
    ADD CONSTRAINT fk_d675c298 FOREIGN KEY (dbguild_id) REFERENCES public.guilds(id);


--
-- TOC entry 3363 (class 2606 OID 25468)
-- Name: toons fk_e9da060c; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.toons
    ADD CONSTRAINT fk_e9da060c FOREIGN KEY (dbgameaccount_id) REFERENCES public.game_accounts(id);


--
-- TOC entry 3342 (class 2606 OID 25473)
-- Name: accounts fk_eac13c47; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accounts
    ADD CONSTRAINT fk_eac13c47 FOREIGN KEY (inviteeaccount_id) REFERENCES public.accounts(id);


--
-- TOC entry 3348 (class 2606 OID 25478)
-- Name: game_accounts fk_edbee460; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.game_accounts
    ADD CONSTRAINT fk_edbee460 FOREIGN KEY (dbaccount_id) REFERENCES public.accounts(id);


--
-- TOC entry 3547 (class 0 OID 0)
-- Dependencies: 5
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: postgres
--

GRANT ALL ON SCHEMA public TO PUBLIC;


-- Completed on 2022-08-18 16:47:30 UTC

--
-- PostgreSQL database dump complete
--

